namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DiscoveryOwnershipState { Draft, Active, Paused, Completed, Archived }
public sealed record DiscoveryOwnershipRecord(Guid Id, string Name, string Owner, DiscoveryOwnershipState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DiscoveryOwnershipCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DiscoveryOwnershipEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DiscoveryOwnershipQuery(string? SearchText, DiscoveryOwnershipState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DiscoveryOwnershipPage(IReadOnlyList<DiscoveryOwnershipRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DiscoveryOwnershipMutation(bool Succeeded, string Code, string Message, DiscoveryOwnershipRecord? Record, DiscoveryOwnershipEvent? Event);
public interface IDiscoveryOwnershipRepository
{
    ValueTask<DiscoveryOwnershipRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DiscoveryOwnershipPage> QueryAsync(DiscoveryOwnershipQuery query, CancellationToken cancellationToken);
    ValueTask<DiscoveryOwnershipMutation> SaveAsync(DiscoveryOwnershipRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDiscoveryOwnershipEventSink { ValueTask PublishAsync(DiscoveryOwnershipEvent domainEvent, CancellationToken cancellationToken); }