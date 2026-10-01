namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DependencyOwnershipState { Draft, Active, Paused, Completed, Archived }
public sealed record DependencyOwnershipRecord(Guid Id, string Name, string Owner, DependencyOwnershipState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DependencyOwnershipCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DependencyOwnershipEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DependencyOwnershipQuery(string? SearchText, DependencyOwnershipState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DependencyOwnershipPage(IReadOnlyList<DependencyOwnershipRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DependencyOwnershipMutation(bool Succeeded, string Code, string Message, DependencyOwnershipRecord? Record, DependencyOwnershipEvent? Event);
public interface IDependencyOwnershipRepository
{
    ValueTask<DependencyOwnershipRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DependencyOwnershipPage> QueryAsync(DependencyOwnershipQuery query, CancellationToken cancellationToken);
    ValueTask<DependencyOwnershipMutation> SaveAsync(DependencyOwnershipRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDependencyOwnershipEventSink { ValueTask PublishAsync(DependencyOwnershipEvent domainEvent, CancellationToken cancellationToken); }