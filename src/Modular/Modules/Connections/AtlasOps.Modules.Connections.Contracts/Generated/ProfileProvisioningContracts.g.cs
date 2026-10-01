namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProfileProvisioningState { Draft, Active, Paused, Completed, Archived }
public sealed record ProfileProvisioningRecord(Guid Id, string Name, string Owner, ProfileProvisioningState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProfileProvisioningCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProfileProvisioningEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProfileProvisioningQuery(string? SearchText, ProfileProvisioningState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProfileProvisioningPage(IReadOnlyList<ProfileProvisioningRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProfileProvisioningMutation(bool Succeeded, string Code, string Message, ProfileProvisioningRecord? Record, ProfileProvisioningEvent? Event);
public interface IProfileProvisioningRepository
{
    ValueTask<ProfileProvisioningRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProfileProvisioningPage> QueryAsync(ProfileProvisioningQuery query, CancellationToken cancellationToken);
    ValueTask<ProfileProvisioningMutation> SaveAsync(ProfileProvisioningRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProfileProvisioningEventSink { ValueTask PublishAsync(ProfileProvisioningEvent domainEvent, CancellationToken cancellationToken); }