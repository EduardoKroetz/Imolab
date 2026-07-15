using Imolab.Exceptions;

namespace Imolab.Domain.Aggregates.ContratoLocacao;

public class CaucaoDinheiro : Caucao
{
    public CaucaoDinheiro(decimal valor, string contaPoupanca)
    {
        Valor = valor;
        ContaPoupanca = contaPoupanca;
    }

    public decimal Valor { get; private set; }
    public string ContaPoupanca { get; private set; }

    private bool Liberada { get; set; } = false;

    public override void Validar(ContratoLocacao contratoLocacao)
    {
        if (Valor <= 0)
            throw new DomainException("O valor da caução em dinheiro deve ser maior que zero.");

        var valorMaximoCaucao = 3 * contratoLocacao.ValorAtualAluguel;

        if (Valor > valorMaximoCaucao)
            throw new DomainException("Caução em dinheiro não pode exceder 3× o valor do aluguel.");
    }

    public override void Liberar(ContratoLocacao contratoLocacao)
    {
        if (contratoLocacao.Status != StatusContrato.Encerrado)
            throw new DomainException("A caução em dinheiro só pode ser liberada quando o contrato estiver encerrado.");

        if (Liberada)
            throw new DomainException("A caução em dinheiro já foi liberada.");

        Liberada = true;

        AddDomainEvent(new CaucaoDinheiroLiberada(Valor, ContaPoupanca));
    }
}
