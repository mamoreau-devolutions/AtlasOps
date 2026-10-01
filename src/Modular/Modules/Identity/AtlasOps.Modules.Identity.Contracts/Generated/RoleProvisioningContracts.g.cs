namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RoleProvisioningState { Draft, Active, Paused, Completed, Archived }
public sealed record RoleProvisioningRecord(Guid Id, string Name, string Owner, RoleProvisioningState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RoleProvisioningCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RoleProvisioningEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RoleProvisioningQuery(string? SearchText, RoleProvisioningState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RoleProvisioningPage(IReadOnlyList<RoleProvisioningRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RoleProvisioningMutation(bool Succeeded, string Code, string Message, RoleProvisioningRecord? Record, RoleProvisioningEvent? Event);
public interface IRoleProvisioningRepository
{
    ValueTask<RoleProvisioningRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RoleProvisioningPage> QueryAsync(RoleProvisioningQuery query, CancellationToken cancellationToken);
    ValueTask<RoleProvisioningMutation> SaveAsync(RoleProvisioningRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRoleProvisioningEventSink { ValueTask PublishAsync(RoleProvisioningEvent domainEvent, CancellationToken cancellationToken); }