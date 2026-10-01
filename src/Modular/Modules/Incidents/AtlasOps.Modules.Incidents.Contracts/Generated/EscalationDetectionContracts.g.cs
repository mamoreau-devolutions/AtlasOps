namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EscalationDetectionState { Draft, Active, Paused, Completed, Archived }
public sealed record EscalationDetectionRecord(Guid Id, string Name, string Owner, EscalationDetectionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EscalationDetectionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EscalationDetectionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EscalationDetectionQuery(string? SearchText, EscalationDetectionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EscalationDetectionPage(IReadOnlyList<EscalationDetectionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EscalationDetectionMutation(bool Succeeded, string Code, string Message, EscalationDetectionRecord? Record, EscalationDetectionEvent? Event);
public interface IEscalationDetectionRepository
{
    ValueTask<EscalationDetectionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EscalationDetectionPage> QueryAsync(EscalationDetectionQuery query, CancellationToken cancellationToken);
    ValueTask<EscalationDetectionMutation> SaveAsync(EscalationDetectionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEscalationDetectionEventSink { ValueTask PublishAsync(EscalationDetectionEvent domainEvent, CancellationToken cancellationToken); }