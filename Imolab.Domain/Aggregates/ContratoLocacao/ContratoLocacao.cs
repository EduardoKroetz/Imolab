using Imolab.Domain.Base;
using Imolab.Exceptions;

namespace Imolab.Domain.Aggregates.ContratoLocacao;

public class ContratoLocacao : Entity
{
    public ContratoLocacao(Guid imovelId, Guid proprietarioId, Guid inquilinoId, decimal valorAluguel, Garantia garantia, DateTime? diaVencimento = null, DateTime? dataInicioVigencia = null, int? prazoMeses = null)
    {
        ImovelId = imovelId;
        LocadorId = proprietarioId;
        LocatarioId = inquilinoId;
        ValorOriginalAluguel = valorAluguel;
        ValorAtualAluguel = valorAluguel;
        Status = StatusContrato.Rascunho;
        DiaVencimento = diaVencimento;
        DataInicioVigencia = dataInicioVigencia;
        PrazoMeses = prazoMeses;
        Garantia = garantia;

        AtualizarDataFimVigencia();
    }

    public Guid ImovelId { get; private set; }
    public Guid LocadorId { get; private set; }
    public Guid LocatarioId { get; private set; }

    public StatusContrato Status { get; private set; }

    public decimal ValorOriginalAluguel { get; private set; }
    public decimal ValorAtualAluguel { get; private set; }

    public DateTime? DiaVencimento { get; private set; }

    public DateTime? DataInicioVigencia { get; private set; }
    public int? PrazoMeses { get; private set; }
    public DateTime? DataFimVigencia { get; private set; }
    private void AtualizarDataFimVigencia()
    {
        if (DataInicioVigencia.HasValue && PrazoMeses.HasValue)
        {
            DataFimVigencia = DataInicioVigencia.Value.AddMonths(PrazoMeses.Value);
        }
        else
        {
            DataFimVigencia = null;
        }
    }

    public Garantia Garantia { get; set; }

    public void AdicionarGarantia(Garantia garantia)
    {
        garantia.Validar(this);

        Garantia = garantia;
    }

    public void Atualizar(decimal valorAluguel, DateTime? diaVencimento = null, DateTime? dataInicioVigencia = null, int? prazoMeses = null)
    {
        if (Status != StatusContrato.Rascunho && Status != StatusContrato.AguardandoVistoriaEntrada)
            throw new DomainException("Contrato não pode ser atualizado neste estado.");

        ValorAtualAluguel = valorAluguel;
        DiaVencimento = diaVencimento;
        DataInicioVigencia = dataInicioVigencia;
        PrazoMeses = prazoMeses;

        AtualizarDataFimVigencia();
    }

    private readonly List<AssinaturaContrato> _assinaturas = [];
    public IReadOnlyCollection<AssinaturaContrato> Assinaturas => _assinaturas.AsReadOnly();

    public void EnviarParaVistoriaEntrada()
    {
        if (Status != StatusContrato.Rascunho)
            throw new DomainException("Contrato não pode ser enviado para vistoria neste estado.");

        Status = StatusContrato.AguardandoVistoriaEntrada;
    }

    public void EnviarParaAssinatura()
    {
        if (Status != StatusContrato.AguardandoVistoriaEntrada)
            throw new DomainException("Contrato não pode ser enviado para assinatura neste estado.");

        if (DiaVencimento is null || DataInicioVigencia is null || PrazoMeses is null || DataFimVigencia is null)
            throw new DomainException("Contrato não pode ser enviado para assinatura sem que todos os campos obrigatórios estejam preenchidos.");

        Status = StatusContrato.AguardandoAssinaturas;
    }

    public void AssinarContrato(TipoParteContrato tipoParte, Guid? responsavelImobiliariaId = null)
    {
        if (_assinaturas.Any(a => a.TipoParte == tipoParte))
            throw new DomainException($"A parte {tipoParte} já assinou o contrato.");

        if (Status != StatusContrato.AguardandoAssinaturas)
            throw new DomainException("Contrato não pode ser assinado neste estado.");

        if (tipoParte == TipoParteContrato.Imobiliaria && responsavelImobiliariaId == null)
            throw new DomainException("É necessário informar o responsável pela imobiliária para assinatura.");

        var parteId = tipoParte switch
        {
            TipoParteContrato.Locador => LocadorId,
            TipoParteContrato.Locatario => LocatarioId,
            TipoParteContrato.Imobiliaria => responsavelImobiliariaId!.Value,
            _ => throw new DomainException("Tipo de parte inválida para assinatura.")
        };

        _assinaturas.Add(new AssinaturaContrato(Id, parteId, tipoParte));

        if (ContratoAssinadoPorTodasPartes())
        {
            Status = StatusContrato.Assinado;
        }
    }

    private bool ContratoAssinadoPorTodasPartes()
    {
        var partesNecessarias = new[] { TipoParteContrato.Locador, TipoParteContrato.Locatario, TipoParteContrato.Imobiliaria };
        var partesAssinaram = _assinaturas.Select(a => a.TipoParte).ToHashSet();

        if (!partesNecessarias.All(p => partesAssinaram.Contains(p)))
            return false;

        return true;
    }

    public void EntregarChavesImovel()
    {
        if (Status != StatusContrato.Assinado)
            throw new DomainException("Chaves do imóvel só podem ser entregues após o contrato ser assinado.");

        Status = StatusContrato.EmExecucao;
    }

    public void EncerrarContrato()
    {
        Garantia.Liberar(this);

        Status = StatusContrato.Encerrado;
    }
}
