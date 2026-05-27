using Imolab.Exceptions;

namespace Imolab.Domain.Aggregates.Vistoria;

public class Vistoria
{
    public Vistoria(Guid contratoLocacaoId, TipoVistoria tipo, Guid? vistoriadorResponsavelId = null)
    {
        Id = Guid.NewGuid();
        ContratoLocacaoId = contratoLocacaoId;
        Tipo = tipo;
        Status = StatusVistoria.Criada;
        VistoriadorResponsavelId = vistoriadorResponsavelId;
    }

    public Guid Id { get; private set; }
    public Guid ContratoLocacaoId { get; private set; }
    public TipoVistoria Tipo { get; private set; }
    public StatusVistoria Status { get; private set; }
    public Guid? VistoriadorResponsavelId { get; private set; }

    public DateTime? AgendadaPra { get; private set; }
    public DateTime? AssinadaEm { get; private set; }
    public DateTime? InspecaoLocalRealizadaEm { get; private set; }

    public LaudoVistoria? Laudo { get; private set; }


    private readonly List<AssinaturaVistoria> _assinaturas = [];
    public IReadOnlyCollection<AssinaturaVistoria> Assinaturas => _assinaturas.AsReadOnly();

    private readonly List<ContestacaoVistoria> _contestacoes = [];
    public IReadOnlyCollection<ContestacaoVistoria> Contestacoes => _contestacoes.AsReadOnly();

    public void Agendar(DateTime dataHora)
    {
        if (Status != StatusVistoria.Criada)
            throw new DomainException("Vistoria só pode ser agendada quando estiver no status 'Criada'.");

        if (dataHora <= DateTime.UtcNow)
            throw new DomainException("Data e hora de agendamento devem ser futuras.");

        AgendadaPra = dataHora;
        Status = StatusVistoria.Agendada;
    }

    public void RegistrarInspecaoLocal(LaudoVistoria laudo, DateTime dataHora)
    {
        if (Status != StatusVistoria.Agendada)
            throw new DomainException("A inspeção local só pode ser registrada quando a vistoria estiver agendada.");

        InspecaoLocalRealizadaEm = dataHora;
        Status = StatusVistoria.InspecaoLocalRealizada;
        Laudo = laudo;
    }

    public void EnviarParaAssinatura()
    {
        if (Status != StatusVistoria.InspecaoLocalRealizada)
            throw new DomainException("O laudo só pode ser enviado após a inspeção local ser registrada.");

        Status = StatusVistoria.AguardandoAssinatura;
    }

    public void RetificarLaudo(LaudoVistoria novoLaudo)
    {
        if (Status is not StatusVistoria.Contestada and not StatusVistoria.InspecaoLocalRealizada and not StatusVistoria.AguardandoAssinatura)
            throw new DomainException("O laudo só pode ser retificado quando a vistoria estiver contestada ou em estado de inspeção local realizada ou aguardando assinatura.");

        _assinaturas.ForEach(a => a.Revogar());

        Laudo = novoLaudo;
        Status = StatusVistoria.InspecaoLocalRealizada;
    }

    public void Assinar(TipoParteVistoria tipoParte, Guid? proprietarioId = null, Guid? inquilinoId = null, Guid? responsavelImobiliariaId = null)
    {
        if (_assinaturas.Any(a => a.TipoParte == tipoParte && a.Ativa))
            throw new DomainException($"A parte {tipoParte} já assinou a vistoria.");

        if (Status != StatusVistoria.AguardandoAssinatura)
            throw new DomainException("Vistoria não pode ser assinada neste estado.");

        var parteId = tipoParte switch
        {
            TipoParteVistoria.Proprietario => proprietarioId,
            TipoParteVistoria.Inquilino => inquilinoId,
            TipoParteVistoria.Imobiliaria => responsavelImobiliariaId,
            _ => throw new DomainException("Tipo de parte inválida para assinatura.")
        };

        if (!parteId.HasValue)
            throw new DomainException($"O ID da parte {tipoParte} é obrigatório para assinatura.");

        _assinaturas.Add(new AssinaturaVistoria(Id, parteId.Value, tipoParte));

        if (VistoriaAssinadaPorTodasPartes())
        {
            Status = StatusVistoria.Assinada;
            AssinadaEm = DateTime.UtcNow;
        }
    }

    private bool VistoriaAssinadaPorTodasPartes()
    {
        var partesNecessarias = new[] { TipoParteVistoria.Proprietario, TipoParteVistoria.Inquilino, TipoParteVistoria.Imobiliaria };
        var partesAssinaram = _assinaturas.Where(a => a.Ativa).Select(a => a.TipoParte).ToHashSet();

        if (!partesNecessarias.All(p => partesAssinaram.Contains(p)))
            return false;

        return true;
    }

    public void Contestar(Guid parteId, TipoParteContestacao tipoParte, string motivo)
    {
        if (Status != StatusVistoria.AguardandoAssinatura)
            throw new DomainException("A vistoria só pode ser contestada enquanto estiver aguardando assinatura.");

        _contestacoes.Add(new ContestacaoVistoria(Id, parteId, tipoParte, motivo));
        Status = StatusVistoria.Contestada;
    }
}

/*
  
TODO: 
- Adicionar status em constestação pra resolver o ponto de contestação nunca ser fechada. 
- Posso ter mais de uma vistoria de entrada válida para mesmo contrato? Não -> validar no domain service

*/
