namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum MembershipRevocationState { Draft, Active, Paused, Completed, Archived }
public sealed record MembershipRevocationRecord(Guid Id, string Name, string Owner, MembershipRevocationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record MembershipRevocationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record MembershipRevocationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record MembershipRevocationQuery(string? SearchText, MembershipRevocationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record MembershipRevocationPage(IReadOnlyList<MembershipRevocationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record MembershipRevocationMutation(bool Succeeded, string Code, string Message, MembershipRevocationRecord? Record, MembershipRevocationEvent? Event);
public interface IMembershipRevocationRepository
{
    ValueTask<MembershipRevocationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<MembershipRevocationPage> QueryAsync(MembershipRevocationQuery query, CancellationToken cancellationToken);
    ValueTask<MembershipRevocationMutation> SaveAsync(MembershipRevocationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IMembershipRevocationEventSink { ValueTask PublishAsync(MembershipRevocationEvent domainEvent, CancellationToken cancellationToken); }