namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProfilePolicyState { Draft, Active, Paused, Completed, Archived }
public sealed record ProfilePolicyRecord(Guid Id, string Name, string Owner, ProfilePolicyState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProfilePolicyCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProfilePolicyEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProfilePolicyQuery(string? SearchText, ProfilePolicyState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProfilePolicyPage(IReadOnlyList<ProfilePolicyRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProfilePolicyMutation(bool Succeeded, string Code, string Message, ProfilePolicyRecord? Record, ProfilePolicyEvent? Event);
public interface IProfilePolicyRepository
{
    ValueTask<ProfilePolicyRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProfilePolicyPage> QueryAsync(ProfilePolicyQuery query, CancellationToken cancellationToken);
    ValueTask<ProfilePolicyMutation> SaveAsync(ProfilePolicyRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProfilePolicyEventSink { ValueTask PublishAsync(ProfilePolicyEvent domainEvent, CancellationToken cancellationToken); }