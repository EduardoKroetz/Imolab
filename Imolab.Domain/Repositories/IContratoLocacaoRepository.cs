using Imolab.Domain.Aggregates.ContratoLocacao;

namespace Imolab.Domain.Repositories;

public interface IContratoLocacaoRepository
{
    Task<ContratoLocacao?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<ContratoLocacao>> ListarContratosAsync();
    Task AdicionarAsync(ContratoLocacao contrato);
    Task AtualizarAsync(ContratoLocacao contrato);
    Task RemoverAsync(ContratoLocacao contrato);
}
