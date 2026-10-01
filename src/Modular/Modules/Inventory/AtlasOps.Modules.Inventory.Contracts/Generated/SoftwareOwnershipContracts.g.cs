namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum SoftwareOwnershipState { Draft, Active, Paused, Completed, Archived }
public sealed record SoftwareOwnershipRecord(Guid Id, string Name, string Owner, SoftwareOwnershipState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record SoftwareOwnershipCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SoftwareOwnershipEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record SoftwareOwnershipQuery(string? SearchText, SoftwareOwnershipState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record SoftwareOwnershipPage(IReadOnlyList<SoftwareOwnershipRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record SoftwareOwnershipMutation(bool Succeeded, string Code, string Message, SoftwareOwnershipRecord? Record, SoftwareOwnershipEvent? Event);
public interface ISoftwareOwnershipRepository
{
    ValueTask<SoftwareOwnershipRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<SoftwareOwnershipPage> QueryAsync(SoftwareOwnershipQuery query, CancellationToken cancellationToken);
    ValueTask<SoftwareOwnershipMutation> SaveAsync(SoftwareOwnershipRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ISoftwareOwnershipEventSink { ValueTask PublishAsync(SoftwareOwnershipEvent domainEvent, CancellationToken cancellationToken); }