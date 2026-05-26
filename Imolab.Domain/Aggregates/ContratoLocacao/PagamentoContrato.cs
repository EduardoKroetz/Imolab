namespace Imolab.Domain.Aggregates.ContratoLocacao;

public class PagamentoContrato
{
    public PagamentoContrato(Guid contratoLocacaoId, TipoPagamentoContrato tipo, decimal valor)
    {
        ContratoLocacaoId = contratoLocacaoId;
        Tipo = tipo;
        Valor = valor;
    }

    public Guid ContratoLocacaoId { get; private set; }
    public TipoPagamentoContrato Tipo { get; private set; }
    public decimal Valor { get; private set; }
}
