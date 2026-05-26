using Imolab.Domain.Aggregates.Vistoria;

namespace Imolab.Domain.Repositories;

public interface IVistoriaRepository
{
    Task<Vistoria> ObterPorIdAsync(Guid id);
    Task AdicionarAsync(Vistoria contrato);
    Task AtualizarAsync(Vistoria contrato);
    Task RemoverAsync(Vistoria contrato);

    Task<bool> ExisteVistoriaEntradaAsync(Guid contratoLocacaoId);

}
