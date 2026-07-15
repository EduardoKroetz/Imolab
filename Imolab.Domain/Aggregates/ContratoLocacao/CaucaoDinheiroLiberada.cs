using Imolab.Domain.Base;

namespace Imolab.Domain.Aggregates.ContratoLocacao;

public record CaucaoDinheiroLiberada : IDomainEvent
{
    public CaucaoDinheiroLiberada(decimal valor, string contaPoupanca)
    {
        Valor = valor;
        ContaPoupanca = contaPoupanca;
    }

    public decimal Valor { get; private set; }
    public string ContaPoupanca { get; private set; }
}
