using Imolab.Exceptions;

namespace Imolab.Domain;

public class ContratoLocacao
{
    public ContratoLocacao(Guid imovelId, Guid proprietarioId, Guid inquilinoId, decimal valorAluguel)
    {
        Id = Guid.NewGuid();
        ImovelId = imovelId;
        ProprietarioId = proprietarioId;
        InquilinoId = inquilinoId;
        ValorAluguel = valorAluguel;
        Status = StatusContratoLocacao.ContratoCriado;
    }

    public Guid Id { get; private set; }
    public Guid ImovelId { get; private set; }
    public Guid ProprietarioId { get; private set; }
    public Guid InquilinoId { get; private set; }

    public StatusContratoLocacao Status { get; private set; }
    public decimal ValorAluguel { get; private set; }
    //public DateTime DataVencimento { get; private set; }
    //public DateTime PrazoContrato { get; private set; }

    private readonly List<Vistoria> _vistorias = [];
    public IReadOnlyCollection<Vistoria> Vistorias => _vistorias.AsReadOnly();

    private readonly List<AssinaturaContrato> _assinaturas = [];
    public IReadOnlyCollection<AssinaturaContrato> Assinaturas => _assinaturas.AsReadOnly();

    private readonly List<PagamentoContrato> _pagamentos = [];
    public IReadOnlyCollection<PagamentoContrato> Pagamentos => _pagamentos.AsReadOnly();

    public void RegistrarVistoria(Vistoria vistoria)
    {
        if (_vistorias.Any(v => v.Id == vistoria.Id))
            throw new DomainException("Vistoria já registrada para este contrato.");

        _vistorias.Add(vistoria);
    }

    public void RegistrarPagamento(PagamentoContrato pagamento)
    {
        if (pagamento.Tipo == TipoPagamento.Aluguel && pagamento.Valor != ValorAluguel)
            throw new DomainException("Valor do pagamento não corresponde ao valor do aluguel.");

        _pagamentos.Add(pagamento);
    }

    public void AssinarContrato(TipoParte tipoParte, Guid? responsavelImobiliariaId = null)
    {
        if (!_vistorias.Any(v => v.Tipo == TipoVistoria.VistoriaEntrada))
            throw new DomainException($"É necessário realizar uma vistoria de entrada antes de assinar o contrato.");

        if (_assinaturas.Any(a => a.TipoParte == tipoParte))
            throw new DomainException($"A parte {tipoParte} já assinou o contrato.");

        if (Status != StatusContratoLocacao.ContratoCriado)
            throw new DomainException("Contrato não pode ser assinado neste estado.");

        if (tipoParte == TipoParte.Imobiliaria && responsavelImobiliariaId == null)
            throw new DomainException("É necessário informar o responsável pela imobiliária para assinatura.");

        var parteId = tipoParte switch
        {
            TipoParte.Proprietario => ProprietarioId,
            TipoParte.Inquilino => InquilinoId,
            TipoParte.Imobiliaria => responsavelImobiliariaId!.Value,
            _ => throw new DomainException("Tipo de parte inválida para assinatura.")
        };

        _assinaturas.Add(new AssinaturaContrato(Id, parteId, tipoParte));

        if (ContratoAssinadoPorTodasPartes())
        {
            Status = StatusContratoLocacao.ContratoAssinado;
        }
    }

    private bool ContratoAssinadoPorTodasPartes()
    {
        var partesNecessarias = new[] { TipoParte.Proprietario, TipoParte.Inquilino, TipoParte.Imobiliaria };
        var partesAssinaram = _assinaturas.Select(a => a.TipoParte).ToHashSet();

        if (!partesNecessarias.All(p => partesAssinaram.Contains(p)))
            return false;

        return true;
    }

    public void EntregarChavesImovel()
    {
        if (Status != StatusContratoLocacao.ContratoAssinado)
            throw new DomainException("Chaves do imóvel só podem ser entregues após o contrato ser assinado.");

        Status = StatusContratoLocacao.EmExecucao;
    }

    public void EncerrarContrato()
    {
        Status = StatusContratoLocacao.ContratoEncerrado;
    }
}

public class PagamentoContrato
{
    public PagamentoContrato(Guid contratoLocacaoId, TipoPagamento tipo, decimal valor)
    {
        ContratoLocacaoId = contratoLocacaoId;
        Tipo = tipo;
        Valor = valor;
    }

    public Guid ContratoLocacaoId { get; private set; }
    public TipoPagamento Tipo { get; private set; }
    public decimal Valor { get; private set; }
}

public enum TipoPagamento
{
    Aluguel = 1
}

// Revisar de faz sentido ser um agregado ou entidade separada
public class AssinaturaContrato
{
    public AssinaturaContrato(Guid contratoLocacaoId, Guid parteId, TipoParte tipoParte)
    {
        ParteId = parteId;
        TipoParte = tipoParte;
        AssinadoEm = DateTime.UtcNow; // Considerar injeção de data/hora
        ContratoLocacaoId = contratoLocacaoId;
    }

    public Guid ParteId { get; private set; }
    public Guid ContratoLocacaoId { get; private set; }
    public TipoParte TipoParte { get; private set; }
    public DateTime AssinadoEm { get; private set; }
}

public enum TipoParte
{
    Proprietario,
    Inquilino,
    Imobiliaria,
    Fiador
}


public class Vistoria
{
    public Vistoria(Guid contratoLocacaoId, string descricao, TipoVistoria tipo)
    {
        ContratoLocacaoId = contratoLocacaoId;
        Descricao = descricao;
        Tipo = tipo;
    }

    public Guid Id { get; private set; }
    public Guid ContratoLocacaoId { get; private set; }
    public TipoVistoria Tipo { get; private set; }
    public string Descricao { get; private set; }
}

public enum TipoVistoria
{
    VistoriaEntrada = 1,
    VistoriaSaida
}

public enum StatusContratoLocacao
{
    ContratoCriado = 1,
    VistoriaEntrada,
    ContratoAssinado,
    EmExecucao,
    InquilinoInadimplente,
    ContinuidadePorIndeterminacao,
    DevolucaoImovel,
    ContratoEncerrado
}
