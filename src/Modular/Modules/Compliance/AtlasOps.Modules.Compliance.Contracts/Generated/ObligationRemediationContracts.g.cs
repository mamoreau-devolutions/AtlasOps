namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ObligationRemediationState { Draft, Active, Paused, Completed, Archived }
public sealed record ObligationRemediationRecord(Guid Id, string Name, string Owner, ObligationRemediationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ObligationRemediationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ObligationRemediationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ObligationRemediationQuery(string? SearchText, ObligationRemediationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ObligationRemediationPage(IReadOnlyList<ObligationRemediationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ObligationRemediationMutation(bool Succeeded, string Code, string Message, ObligationRemediationRecord? Record, ObligationRemediationEvent? Event);
public interface IObligationRemediationRepository
{
    ValueTask<ObligationRemediationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ObligationRemediationPage> QueryAsync(ObligationRemediationQuery query, CancellationToken cancellationToken);
    ValueTask<ObligationRemediationMutation> SaveAsync(ObligationRemediationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IObligationRemediationEventSink { ValueTask PublishAsync(ObligationRemediationEvent domainEvent, CancellationToken cancellationToken); }