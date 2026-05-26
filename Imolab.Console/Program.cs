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

var contrato = new ContratoLocacao(
    imovelId: Guid.NewGuid(),
    proprietarioId,
    inquilinoId,
    valorAluguel
);

await contratoLocacaoRepository.AdicionarAsync(contrato);

var vistoria = new Vistoria(
    contratoLocacaoId: contrato.Id,
    descricao: "Vistoria inicial do imóvel.",
    tipo: TipoVistoria.VistoriaEntrada
);

await vistoriaRepository.AdicionarAsync(vistoria);

await assinaturaContratoService.ValidarVistoriaEntrada(contrato.Id);

contrato.AssinarContrato(TipoParteContrato.Proprietario);
contrato.AssinarContrato(TipoParteContrato.Inquilino);
contrato.AssinarContrato(TipoParteContrato.Imobiliaria, responsavelImobiliariaId);

var pagamentoAluguel = new PagamentoContrato(
    contratoLocacaoId: contrato.Id,
    tipo: TipoPagamentoContrato.Aluguel,
    valor: valorAluguel
);

contrato.EntregarChavesImovel();

contrato.RegistrarPagamento(pagamentoAluguel);

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

    public Task<bool> ExisteVistoriaEntradaAsync(Guid contratoLocacaoId)
    {
        return _vistorias.Any(v => v.ContratoLocacaoId == contratoLocacaoId && v.Tipo == TipoVistoria.VistoriaEntrada)
            ? Task.FromResult(true)
            : Task.FromResult(false);
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



