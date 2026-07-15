using Imolab.Domain.Base;

namespace Imolab.Domain.Aggregates.ContratoLocacao;

public abstract class Garantia : Entity
{
    public abstract void Validar(ContratoLocacao contratoLocacao);
    public abstract void Liberar(ContratoLocacao contratoLocacao);
}
