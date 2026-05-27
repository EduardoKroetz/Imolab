namespace Imolab.Domain.Aggregates.Vistoria;

public class AssinaturaVistoria
{
    public AssinaturaVistoria(Guid vistoriaId, Guid parteId, TipoParteVistoria tipoParte)
    {
        Id = Guid.NewGuid();
        VistoriaId = vistoriaId;
        ParteId = parteId;
        TipoParte = tipoParte;
        AssinadoEm = DateTime.UtcNow;
        Ativa = true;
    }

    public Guid Id { get; private set; }
    public Guid VistoriaId { get; private set; }
    public Guid ParteId { get; private set; }
    public TipoParteVistoria TipoParte { get; private set; }
    public DateTime AssinadoEm { get; private set; }
    public bool Ativa { get; private set; }

    public void Revogar()
    {
        Ativa = false;
    }
}
