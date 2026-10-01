namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProfileRecoveryState { Draft, Active, Paused, Completed, Archived }
public sealed record ProfileRecoveryRecord(Guid Id, string Name, string Owner, ProfileRecoveryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProfileRecoveryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProfileRecoveryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProfileRecoveryQuery(string? SearchText, ProfileRecoveryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProfileRecoveryPage(IReadOnlyList<ProfileRecoveryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProfileRecoveryMutation(bool Succeeded, string Code, string Message, ProfileRecoveryRecord? Record, ProfileRecoveryEvent? Event);
public interface IProfileRecoveryRepository
{
    ValueTask<ProfileRecoveryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProfileRecoveryPage> QueryAsync(ProfileRecoveryQuery query, CancellationToken cancellationToken);
    ValueTask<ProfileRecoveryMutation> SaveAsync(ProfileRecoveryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProfileRecoveryEventSink { ValueTask PublishAsync(ProfileRecoveryEvent domainEvent, CancellationToken cancellationToken); }