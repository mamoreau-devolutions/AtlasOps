namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum MembershipProvisioningState { Draft, Active, Paused, Completed, Archived }
public sealed record MembershipProvisioningRecord(Guid Id, string Name, string Owner, MembershipProvisioningState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record MembershipProvisioningCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record MembershipProvisioningEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record MembershipProvisioningQuery(string? SearchText, MembershipProvisioningState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record MembershipProvisioningPage(IReadOnlyList<MembershipProvisioningRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record MembershipProvisioningMutation(bool Succeeded, string Code, string Message, MembershipProvisioningRecord? Record, MembershipProvisioningEvent? Event);
public interface IMembershipProvisioningRepository
{
    ValueTask<MembershipProvisioningRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<MembershipProvisioningPage> QueryAsync(MembershipProvisioningQuery query, CancellationToken cancellationToken);
    ValueTask<MembershipProvisioningMutation> SaveAsync(MembershipProvisioningRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IMembershipProvisioningEventSink { ValueTask PublishAsync(MembershipProvisioningEvent domainEvent, CancellationToken cancellationToken); }