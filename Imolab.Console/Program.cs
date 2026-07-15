using Imolab.Domain.Aggregates.ContratoLocacao;
using Imolab.Domain.Aggregates.Vistoria;
using Imolab.Domain.DomainServices;
using Imolab.Domain.Repositories;


var contratoLocacaoRepository = new ContratoLocacaoRepository();
var vistoriaRepository = new VistoriaRepository();

var assinaturaContratoService = new AssinaturaContratoService(vistoriaRepository);

var proprietarioId = Guid.NewGuid();
var inquilinoId = Guid.NewGuid();
var responsavelImobiliariaId = Guid.NewGuid();
var valorAluguel = 1500m;

var caucaoDinheiro = new CaucaoDinheiro(valor: 2000m, contaPoupanca: "12345-6");

var contrato = new ContratoLocacao(
    imovelId: Guid.NewGuid(),
    proprietarioId,
    inquilinoId,
    valorAluguel,
    caucaoDinheiro
);

contrato.EnviarParaVistoriaEntrada();

await contratoLocacaoRepository.AdicionarAsync(contrato);

var vistoria = new Vistoria(
    contratoLocacaoId: contrato.Id,
    tipo: TipoVistoria.VistoriaEntrada);

vistoria.Agendar(dataHora: DateTime.UtcNow.AddDays(1));

var laudoVistoria = new LaudoVistoria(valor: 125);

vistoria.RegistrarInspecaoLocal(
    laudo: laudoVistoria,
    dataHora: DateTime.UtcNow.AddHours(-2));

vistoria.EnviarParaAssinatura();

vistoria.Assinar(TipoParteVistoria.Proprietario, proprietarioId);
vistoria.Assinar(TipoParteVistoria.Inquilino, inquilinoId);
vistoria.Assinar(TipoParteVistoria.Imobiliaria, responsavelImobiliariaId);

await vistoriaRepository.AdicionarAsync(vistoria);

await assinaturaContratoService.ValidarVistoriaEntradaAssinada(contrato.Id);

contrato.Atualizar(valorAluguel: 1200, diaVencimento: DateTime.UtcNow.AddMonths(10), dataInicioVigencia: DateTime.UtcNow.AddDays(2), prazoMeses: 10);

contrato.EnviarParaAssinatura();

contrato.AssinarContrato(TipoParteContrato.Locador);
contrato.AssinarContrato(TipoParteContrato.Locatario);
contrato.AssinarContrato(TipoParteContrato.Imobiliaria, responsavelImobiliariaId);

contrato.EntregarChavesImovel();

contrato.EncerrarContrato();


public class ContratoLocacaoRepository : IContratoLocacaoRepository
{
    private List<ContratoLocacao> _contratosLocacoes = [];

    public Task AdicionarAsync(ContratoLocacao contrato)
    {
        _contratosLocacoes.Add(contrato);
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(ContratoLocacao contrato)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ContratoLocacao>> ListarContratosAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ContratoLocacao?> ObterPorIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task RemoverAsync(ContratoLocacao contrato)
    {
        throw new NotImplementedException();
    }
}

public class VistoriaRepository : IVistoriaRepository
{
    private List<Vistoria> _vistorias = [];
    public Task AdicionarAsync(Vistoria vistoria)
    {
        _vistorias.Add(vistoria);
        return Task.CompletedTask;
    }


    public Task<List<Vistoria>> ObterListaVistoriasPorContratoIdAsync(Guid contratoLocacaoId)
    {
        return Task.FromResult(_vistorias.Where(v => v.ContratoLocacaoId == contratoLocacaoId).ToList());
    }

    public Task AtualizarAsync(Vistoria vistoria)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Vistoria>> ListarVistoriasAsync()
    {
        throw new NotImplementedException();
    }
    public Task<Vistoria> ObterPorIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
    public Task RemoverAsync(Vistoria vistoria)
    {
        throw new NotImplementedException();
    }
}



