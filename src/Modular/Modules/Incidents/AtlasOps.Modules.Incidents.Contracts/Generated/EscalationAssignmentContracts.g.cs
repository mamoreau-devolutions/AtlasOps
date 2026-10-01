namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EscalationAssignmentState { Draft, Active, Paused, Completed, Archived }
public sealed record EscalationAssignmentRecord(Guid Id, string Name, string Owner, EscalationAssignmentState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EscalationAssignmentCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EscalationAssignmentEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EscalationAssignmentQuery(string? SearchText, EscalationAssignmentState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EscalationAssignmentPage(IReadOnlyList<EscalationAssignmentRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EscalationAssignmentMutation(bool Succeeded, string Code, string Message, EscalationAssignmentRecord? Record, EscalationAssignmentEvent? Event);
public interface IEscalationAssignmentRepository
{
    ValueTask<EscalationAssignmentRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EscalationAssignmentPage> QueryAsync(EscalationAssignmentQuery query, CancellationToken cancellationToken);
    ValueTask<EscalationAssignmentMutation> SaveAsync(EscalationAssignmentRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEscalationAssignmentEventSink { ValueTask PublishAsync(EscalationAssignmentEvent domainEvent, CancellationToken cancellationToken); }