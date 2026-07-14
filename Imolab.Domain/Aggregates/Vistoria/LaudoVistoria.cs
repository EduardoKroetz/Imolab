namespace Imolab.Domain.Aggregates.Vistoria;

public class LaudoVistoria
{
    public LaudoVistoria(decimal valor)
    {
        Valor = valor;
    }

    public decimal Valor { get; private set; }

}
