namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EvidenceAuthoringState { Draft, Active, Paused, Completed, Archived }
public sealed record EvidenceAuthoringRecord(Guid Id, string Name, string Owner, EvidenceAuthoringState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EvidenceAuthoringCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EvidenceAuthoringEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EvidenceAuthoringQuery(string? SearchText, EvidenceAuthoringState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EvidenceAuthoringPage(IReadOnlyList<EvidenceAuthoringRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EvidenceAuthoringMutation(bool Succeeded, string Code, string Message, EvidenceAuthoringRecord? Record, EvidenceAuthoringEvent? Event);
public interface IEvidenceAuthoringRepository
{
    ValueTask<EvidenceAuthoringRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EvidenceAuthoringPage> QueryAsync(EvidenceAuthoringQuery query, CancellationToken cancellationToken);
    ValueTask<EvidenceAuthoringMutation> SaveAsync(EvidenceAuthoringRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEvidenceAuthoringEventSink { ValueTask PublishAsync(EvidenceAuthoringEvent domainEvent, CancellationToken cancellationToken); }