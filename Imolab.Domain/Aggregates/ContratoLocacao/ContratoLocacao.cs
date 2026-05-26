using Imolab.Exceptions;

namespace Imolab.Domain.Aggregates.ContratoLocacao;

public class ContratoLocacao
{
    public ContratoLocacao(Guid imovelId, Guid proprietarioId, Guid inquilinoId, decimal valorAluguel)
    {
        Id = Guid.NewGuid();
        ImovelId = imovelId;
        ProprietarioId = proprietarioId;
        InquilinoId = inquilinoId;
        ValorAluguel = valorAluguel;
        Status = StatusContrato.ContratoCriado;
    }

    public Guid Id { get; private set; }
    public Guid ImovelId { get; private set; }
    public Guid ProprietarioId { get; private set; }
    public Guid InquilinoId { get; private set; }

    public StatusContrato Status { get; private set; }
    public decimal ValorAluguel { get; private set; }


    private readonly List<AssinaturaContrato> _assinaturas = [];
    public IReadOnlyCollection<AssinaturaContrato> Assinaturas => _assinaturas.AsReadOnly();

    private readonly List<PagamentoContrato> _pagamentos = [];
    public IReadOnlyCollection<PagamentoContrato> Pagamentos => _pagamentos.AsReadOnly();

    public void RegistrarPagamento(PagamentoContrato pagamento)
    {
        if (pagamento.Tipo == TipoPagamentoContrato.Aluguel && pagamento.Valor != ValorAluguel)
            throw new DomainException("Valor do pagamento não corresponde ao valor do aluguel.");

        _pagamentos.Add(pagamento);
    }

    public void AssinarContrato(TipoParteContrato tipoParte, Guid? responsavelImobiliariaId = null)
    {
        if (_assinaturas.Any(a => a.TipoParte == tipoParte))
            throw new DomainException($"A parte {tipoParte} já assinou o contrato.");

        if (Status != StatusContrato.ContratoCriado)
            throw new DomainException("Contrato não pode ser assinado neste estado.");

        if (tipoParte == TipoParteContrato.Imobiliaria && responsavelImobiliariaId == null)
            throw new DomainException("É necessário informar o responsável pela imobiliária para assinatura.");

        var parteId = tipoParte switch
        {
            TipoParteContrato.Proprietario => ProprietarioId,
            TipoParteContrato.Inquilino => InquilinoId,
            TipoParteContrato.Imobiliaria => responsavelImobiliariaId!.Value,
            _ => throw new DomainException("Tipo de parte inválida para assinatura.")
        };

        _assinaturas.Add(new AssinaturaContrato(Id, parteId, tipoParte));

        if (ContratoAssinadoPorTodasPartes())
        {
            Status = StatusContrato.ContratoAssinado;
        }
    }

    private bool ContratoAssinadoPorTodasPartes()
    {
        var partesNecessarias = new[] { TipoParteContrato.Proprietario, TipoParteContrato.Inquilino, TipoParteContrato.Imobiliaria };
        var partesAssinaram = _assinaturas.Select(a => a.TipoParte).ToHashSet();

        if (!partesNecessarias.All(p => partesAssinaram.Contains(p)))
            return false;

        return true;
    }

    public void EntregarChavesImovel()
    {
        if (Status != StatusContrato.ContratoAssinado)
            throw new DomainException("Chaves do imóvel só podem ser entregues após o contrato ser assinado.");

        Status = StatusContrato.EmExecucao;
    }

    public void EncerrarContrato()
    {
        Status = StatusContrato.ContratoEncerrado;
    }
}
