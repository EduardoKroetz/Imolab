namespace Imolab.Domain.Aggregates.Vistoria;

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

