namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EscalationResolutionState { Draft, Active, Paused, Completed, Archived }
public sealed record EscalationResolutionRecord(Guid Id, string Name, string Owner, EscalationResolutionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EscalationResolutionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EscalationResolutionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EscalationResolutionQuery(string? SearchText, EscalationResolutionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EscalationResolutionPage(IReadOnlyList<EscalationResolutionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EscalationResolutionMutation(bool Succeeded, string Code, string Message, EscalationResolutionRecord? Record, EscalationResolutionEvent? Event);
public interface IEscalationResolutionRepository
{
    ValueTask<EscalationResolutionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EscalationResolutionPage> QueryAsync(EscalationResolutionQuery query, CancellationToken cancellationToken);
    ValueTask<EscalationResolutionMutation> SaveAsync(EscalationResolutionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEscalationResolutionEventSink { ValueTask PublishAsync(EscalationResolutionEvent domainEvent, CancellationToken cancellationToken); }