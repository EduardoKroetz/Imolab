namespace Imolab.Domain.Base;

public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent e) => _domainEvents.Add(e);
    public void ClearEvents() => _domainEvents.Clear();
}

