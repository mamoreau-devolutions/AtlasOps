namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TriggerDesignState { Draft, Active, Paused, Completed, Archived }
public sealed record TriggerDesignRecord(Guid Id, string Name, string Owner, TriggerDesignState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TriggerDesignCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TriggerDesignEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TriggerDesignQuery(string? SearchText, TriggerDesignState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TriggerDesignPage(IReadOnlyList<TriggerDesignRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TriggerDesignMutation(bool Succeeded, string Code, string Message, TriggerDesignRecord? Record, TriggerDesignEvent? Event);
public interface ITriggerDesignRepository
{
    ValueTask<TriggerDesignRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TriggerDesignPage> QueryAsync(TriggerDesignQuery query, CancellationToken cancellationToken);
    ValueTask<TriggerDesignMutation> SaveAsync(TriggerDesignRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITriggerDesignEventSink { ValueTask PublishAsync(TriggerDesignEvent domainEvent, CancellationToken cancellationToken); }