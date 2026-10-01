namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EvidenceAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record EvidenceAuditRecord(Guid Id, string Name, string Owner, EvidenceAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EvidenceAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EvidenceAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EvidenceAuditQuery(string? SearchText, EvidenceAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EvidenceAuditPage(IReadOnlyList<EvidenceAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EvidenceAuditMutation(bool Succeeded, string Code, string Message, EvidenceAuditRecord? Record, EvidenceAuditEvent? Event);
public interface IEvidenceAuditRepository
{
    ValueTask<EvidenceAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EvidenceAuditPage> QueryAsync(EvidenceAuditQuery query, CancellationToken cancellationToken);
    ValueTask<EvidenceAuditMutation> SaveAsync(EvidenceAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEvidenceAuditEventSink { ValueTask PublishAsync(EvidenceAuditEvent domainEvent, CancellationToken cancellationToken); }