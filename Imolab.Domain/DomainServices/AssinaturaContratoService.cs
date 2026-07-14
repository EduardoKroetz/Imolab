using Imolab.Domain.Aggregates.Vistoria;
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

    public async Task ValidarVistoriaEntradaAssinada(Guid contratoLocacaoId)
    {
        var vistorias = await _vistoriaRepository.ObterListaVistoriasPorContratoIdAsync(contratoLocacaoId);

        var entradasAssinadas = vistorias
            .Where(v => v.Tipo == TipoVistoria.VistoriaEntrada && v.Status == StatusVistoria.Assinada)
            .ToList();

        if (entradasAssinadas.Count == 0)
            throw new DomainException("É necessário uma vistoria de entrada assinada antes de enviar para assinatura.");

        if (entradasAssinadas.Count > 1)
            throw new DomainException("Existe mais de uma vistoria de entrada assinada para o mesmo contrato — estado inconsistente.");
    }

}
