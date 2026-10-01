namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ObligationDetectionState { Draft, Active, Paused, Completed, Archived }
public sealed record ObligationDetectionRecord(Guid Id, string Name, string Owner, ObligationDetectionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ObligationDetectionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ObligationDetectionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ObligationDetectionQuery(string? SearchText, ObligationDetectionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ObligationDetectionPage(IReadOnlyList<ObligationDetectionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ObligationDetectionMutation(bool Succeeded, string Code, string Message, ObligationDetectionRecord? Record, ObligationDetectionEvent? Event);
public interface IObligationDetectionRepository
{
    ValueTask<ObligationDetectionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ObligationDetectionPage> QueryAsync(ObligationDetectionQuery query, CancellationToken cancellationToken);
    ValueTask<ObligationDetectionMutation> SaveAsync(ObligationDetectionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IObligationDetectionEventSink { ValueTask PublishAsync(ObligationDetectionEvent domainEvent, CancellationToken cancellationToken); }