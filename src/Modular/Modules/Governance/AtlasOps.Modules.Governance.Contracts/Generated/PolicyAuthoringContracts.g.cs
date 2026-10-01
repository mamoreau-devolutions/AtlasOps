namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PolicyAuthoringState { Draft, Active, Paused, Completed, Archived }
public sealed record PolicyAuthoringRecord(Guid Id, string Name, string Owner, PolicyAuthoringState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PolicyAuthoringCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PolicyAuthoringEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PolicyAuthoringQuery(string? SearchText, PolicyAuthoringState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PolicyAuthoringPage(IReadOnlyList<PolicyAuthoringRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PolicyAuthoringMutation(bool Succeeded, string Code, string Message, PolicyAuthoringRecord? Record, PolicyAuthoringEvent? Event);
public interface IPolicyAuthoringRepository
{
    ValueTask<PolicyAuthoringRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PolicyAuthoringPage> QueryAsync(PolicyAuthoringQuery query, CancellationToken cancellationToken);
    ValueTask<PolicyAuthoringMutation> SaveAsync(PolicyAuthoringRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPolicyAuthoringEventSink { ValueTask PublishAsync(PolicyAuthoringEvent domainEvent, CancellationToken cancellationToken); }