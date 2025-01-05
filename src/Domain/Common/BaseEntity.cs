﻿using System.ComponentModel.DataAnnotations.Schema;

namespace TP.Domain.Common;

public abstract class BaseEntity<T>
{
    public T? Id { get; set; } //todo: should be used imutable => set -> private set
    public DateTime CreatedAt { get; protected set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; protected set; }
    public T CreatedBy { get; protected set; }
    public T? UpdatedBy { get; protected set; }
    public bool IsDeleted { get; protected set; }
}

public abstract class BaseEntity : BaseEntity<int>
{
    private readonly List<BaseEvent> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
