namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EvidenceEnforcementState { Draft, Active, Paused, Completed, Archived }
public sealed record EvidenceEnforcementRecord(Guid Id, string Name, string Owner, EvidenceEnforcementState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EvidenceEnforcementCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EvidenceEnforcementEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EvidenceEnforcementQuery(string? SearchText, EvidenceEnforcementState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EvidenceEnforcementPage(IReadOnlyList<EvidenceEnforcementRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EvidenceEnforcementMutation(bool Succeeded, string Code, string Message, EvidenceEnforcementRecord? Record, EvidenceEnforcementEvent? Event);
public interface IEvidenceEnforcementRepository
{
    ValueTask<EvidenceEnforcementRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EvidenceEnforcementPage> QueryAsync(EvidenceEnforcementQuery query, CancellationToken cancellationToken);
    ValueTask<EvidenceEnforcementMutation> SaveAsync(EvidenceEnforcementRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEvidenceEnforcementEventSink { ValueTask PublishAsync(EvidenceEnforcementEvent domainEvent, CancellationToken cancellationToken); }