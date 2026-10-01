namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ControlAuthoringState { Draft, Active, Paused, Completed, Archived }
public sealed record ControlAuthoringRecord(Guid Id, string Name, string Owner, ControlAuthoringState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ControlAuthoringCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ControlAuthoringEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ControlAuthoringQuery(string? SearchText, ControlAuthoringState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ControlAuthoringPage(IReadOnlyList<ControlAuthoringRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ControlAuthoringMutation(bool Succeeded, string Code, string Message, ControlAuthoringRecord? Record, ControlAuthoringEvent? Event);
public interface IControlAuthoringRepository
{
    ValueTask<ControlAuthoringRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ControlAuthoringPage> QueryAsync(ControlAuthoringQuery query, CancellationToken cancellationToken);
    ValueTask<ControlAuthoringMutation> SaveAsync(ControlAuthoringRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IControlAuthoringEventSink { ValueTask PublishAsync(ControlAuthoringEvent domainEvent, CancellationToken cancellationToken); }