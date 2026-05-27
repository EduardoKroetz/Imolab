namespace Imolab.Domain.Aggregates.Vistoria;

public class ContestacaoVistoria
{
    public ContestacaoVistoria(Guid vistoriaId, Guid parteId, TipoParteContestacao tipoParte, string motivo)
    {
        Id = Guid.NewGuid();
        VistoriaId = vistoriaId;
        ParteId = parteId;
        TipoParte = tipoParte;
        Motivo = motivo;
    }

    public Guid Id { get; private set; }
    public Guid VistoriaId { get; private set; }
    public Guid ParteId { get; private set; }
    public TipoParteContestacao TipoParte { get; private set; }
    public string Motivo { get; private set; }
}
