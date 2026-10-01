namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RoleRevocationState { Draft, Active, Paused, Completed, Archived }
public sealed record RoleRevocationRecord(Guid Id, string Name, string Owner, RoleRevocationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RoleRevocationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RoleRevocationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RoleRevocationQuery(string? SearchText, RoleRevocationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RoleRevocationPage(IReadOnlyList<RoleRevocationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RoleRevocationMutation(bool Succeeded, string Code, string Message, RoleRevocationRecord? Record, RoleRevocationEvent? Event);
public interface IRoleRevocationRepository
{
    ValueTask<RoleRevocationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RoleRevocationPage> QueryAsync(RoleRevocationQuery query, CancellationToken cancellationToken);
    ValueTask<RoleRevocationMutation> SaveAsync(RoleRevocationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRoleRevocationEventSink { ValueTask PublishAsync(RoleRevocationEvent domainEvent, CancellationToken cancellationToken); }