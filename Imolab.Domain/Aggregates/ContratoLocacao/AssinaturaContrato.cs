namespace Imolab.Domain.Aggregates.ContratoLocacao;

public class AssinaturaContrato
{
    public AssinaturaContrato(Guid contratoLocacaoId, Guid parteId, TipoParteContrato tipoParte)
    {
        ParteId = parteId;
        TipoParte = tipoParte;
        AssinadoEm = DateTime.UtcNow; // TODO: Refatorar para usar provedor de data/hora para facilitar testes futuramente
        ContratoLocacaoId = contratoLocacaoId;
    }

    public Guid ParteId { get; private set; }
    public Guid ContratoLocacaoId { get; private set; }
    public TipoParteContrato TipoParte { get; private set; }
    public DateTime AssinadoEm { get; private set; }
}