namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProfileHealthState { Draft, Active, Paused, Completed, Archived }
public sealed record ProfileHealthRecord(Guid Id, string Name, string Owner, ProfileHealthState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProfileHealthCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProfileHealthEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProfileHealthQuery(string? SearchText, ProfileHealthState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProfileHealthPage(IReadOnlyList<ProfileHealthRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProfileHealthMutation(bool Succeeded, string Code, string Message, ProfileHealthRecord? Record, ProfileHealthEvent? Event);
public interface IProfileHealthRepository
{
    ValueTask<ProfileHealthRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProfileHealthPage> QueryAsync(ProfileHealthQuery query, CancellationToken cancellationToken);
    ValueTask<ProfileHealthMutation> SaveAsync(ProfileHealthRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProfileHealthEventSink { ValueTask PublishAsync(ProfileHealthEvent domainEvent, CancellationToken cancellationToken); }