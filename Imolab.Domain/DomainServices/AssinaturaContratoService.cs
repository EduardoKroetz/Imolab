using Imolab.Domain.Repositories;
using Imolab.Exceptions;

namespace Imolab.Domain.DomainServices;

public class AssinaturaContratoService
{
    private readonly IVistoriaRepository _vistoriaRepository;

    public AssinaturaContratoService(IVistoriaRepository vistoriaRepository)
    {
        _vistoriaRepository = vistoriaRepository;
    }

    public async Task ValidarVistoriaEntrada(Guid contratoLocacaoId)
    {
        var existeVistoriaEntrada = await _vistoriaRepository.ExisteVistoriaEntradaAsync(contratoLocacaoId);

        if (!existeVistoriaEntrada)
            throw new DomainException("É necessário realizar uma vistoria de entrada antes de assinar o contrato.");
    }

}
