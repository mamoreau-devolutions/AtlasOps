namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum SessionProvisioningState { Draft, Active, Paused, Completed, Archived }
public sealed record SessionProvisioningRecord(Guid Id, string Name, string Owner, SessionProvisioningState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record SessionProvisioningCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SessionProvisioningEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record SessionProvisioningQuery(string? SearchText, SessionProvisioningState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record SessionProvisioningPage(IReadOnlyList<SessionProvisioningRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record SessionProvisioningMutation(bool Succeeded, string Code, string Message, SessionProvisioningRecord? Record, SessionProvisioningEvent? Event);
public interface ISessionProvisioningRepository
{
    ValueTask<SessionProvisioningRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<SessionProvisioningPage> QueryAsync(SessionProvisioningQuery query, CancellationToken cancellationToken);
    ValueTask<SessionProvisioningMutation> SaveAsync(SessionProvisioningRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ISessionProvisioningEventSink { ValueTask PublishAsync(SessionProvisioningEvent domainEvent, CancellationToken cancellationToken); }