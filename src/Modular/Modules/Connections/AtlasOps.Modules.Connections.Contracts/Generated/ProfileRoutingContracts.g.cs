namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProfileRoutingState { Draft, Active, Paused, Completed, Archived }
public sealed record ProfileRoutingRecord(Guid Id, string Name, string Owner, ProfileRoutingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProfileRoutingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProfileRoutingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProfileRoutingQuery(string? SearchText, ProfileRoutingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProfileRoutingPage(IReadOnlyList<ProfileRoutingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProfileRoutingMutation(bool Succeeded, string Code, string Message, ProfileRoutingRecord? Record, ProfileRoutingEvent? Event);
public interface IProfileRoutingRepository
{
    ValueTask<ProfileRoutingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProfileRoutingPage> QueryAsync(ProfileRoutingQuery query, CancellationToken cancellationToken);
    ValueTask<ProfileRoutingMutation> SaveAsync(ProfileRoutingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProfileRoutingEventSink { ValueTask PublishAsync(ProfileRoutingEvent domainEvent, CancellationToken cancellationToken); }