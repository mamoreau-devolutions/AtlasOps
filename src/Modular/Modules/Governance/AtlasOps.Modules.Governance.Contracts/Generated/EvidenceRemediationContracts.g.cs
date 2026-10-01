namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EvidenceRemediationState { Draft, Active, Paused, Completed, Archived }
public sealed record EvidenceRemediationRecord(Guid Id, string Name, string Owner, EvidenceRemediationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EvidenceRemediationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EvidenceRemediationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EvidenceRemediationQuery(string? SearchText, EvidenceRemediationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EvidenceRemediationPage(IReadOnlyList<EvidenceRemediationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EvidenceRemediationMutation(bool Succeeded, string Code, string Message, EvidenceRemediationRecord? Record, EvidenceRemediationEvent? Event);
public interface IEvidenceRemediationRepository
{
    ValueTask<EvidenceRemediationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EvidenceRemediationPage> QueryAsync(EvidenceRemediationQuery query, CancellationToken cancellationToken);
    ValueTask<EvidenceRemediationMutation> SaveAsync(EvidenceRemediationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEvidenceRemediationEventSink { ValueTask PublishAsync(EvidenceRemediationEvent domainEvent, CancellationToken cancellationToken); }