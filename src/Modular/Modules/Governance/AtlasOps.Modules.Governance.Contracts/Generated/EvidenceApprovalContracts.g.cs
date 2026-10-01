namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EvidenceApprovalState { Draft, Active, Paused, Completed, Archived }
public sealed record EvidenceApprovalRecord(Guid Id, string Name, string Owner, EvidenceApprovalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EvidenceApprovalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EvidenceApprovalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EvidenceApprovalQuery(string? SearchText, EvidenceApprovalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EvidenceApprovalPage(IReadOnlyList<EvidenceApprovalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EvidenceApprovalMutation(bool Succeeded, string Code, string Message, EvidenceApprovalRecord? Record, EvidenceApprovalEvent? Event);
public interface IEvidenceApprovalRepository
{
    ValueTask<EvidenceApprovalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EvidenceApprovalPage> QueryAsync(EvidenceApprovalQuery query, CancellationToken cancellationToken);
    ValueTask<EvidenceApprovalMutation> SaveAsync(EvidenceApprovalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEvidenceApprovalEventSink { ValueTask PublishAsync(EvidenceApprovalEvent domainEvent, CancellationToken cancellationToken); }