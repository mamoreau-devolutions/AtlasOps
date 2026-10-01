namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DiscoveryClassificationState { Draft, Active, Paused, Completed, Archived }
public sealed record DiscoveryClassificationRecord(Guid Id, string Name, string Owner, DiscoveryClassificationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DiscoveryClassificationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DiscoveryClassificationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DiscoveryClassificationQuery(string? SearchText, DiscoveryClassificationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DiscoveryClassificationPage(IReadOnlyList<DiscoveryClassificationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DiscoveryClassificationMutation(bool Succeeded, string Code, string Message, DiscoveryClassificationRecord? Record, DiscoveryClassificationEvent? Event);
public interface IDiscoveryClassificationRepository
{
    ValueTask<DiscoveryClassificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DiscoveryClassificationPage> QueryAsync(DiscoveryClassificationQuery query, CancellationToken cancellationToken);
    ValueTask<DiscoveryClassificationMutation> SaveAsync(DiscoveryClassificationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDiscoveryClassificationEventSink { ValueTask PublishAsync(DiscoveryClassificationEvent domainEvent, CancellationToken cancellationToken); }