namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DiscoveryDriftState { Draft, Active, Paused, Completed, Archived }
public sealed record DiscoveryDriftRecord(Guid Id, string Name, string Owner, DiscoveryDriftState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DiscoveryDriftCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DiscoveryDriftEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DiscoveryDriftQuery(string? SearchText, DiscoveryDriftState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DiscoveryDriftPage(IReadOnlyList<DiscoveryDriftRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DiscoveryDriftMutation(bool Succeeded, string Code, string Message, DiscoveryDriftRecord? Record, DiscoveryDriftEvent? Event);
public interface IDiscoveryDriftRepository
{
    ValueTask<DiscoveryDriftRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DiscoveryDriftPage> QueryAsync(DiscoveryDriftQuery query, CancellationToken cancellationToken);
    ValueTask<DiscoveryDriftMutation> SaveAsync(DiscoveryDriftRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDiscoveryDriftEventSink { ValueTask PublishAsync(DiscoveryDriftEvent domainEvent, CancellationToken cancellationToken); }