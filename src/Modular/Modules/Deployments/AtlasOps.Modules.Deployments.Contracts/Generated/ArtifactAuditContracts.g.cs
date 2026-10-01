namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ArtifactAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record ArtifactAuditRecord(Guid Id, string Name, string Owner, ArtifactAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ArtifactAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ArtifactAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ArtifactAuditQuery(string? SearchText, ArtifactAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ArtifactAuditPage(IReadOnlyList<ArtifactAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ArtifactAuditMutation(bool Succeeded, string Code, string Message, ArtifactAuditRecord? Record, ArtifactAuditEvent? Event);
public interface IArtifactAuditRepository
{
    ValueTask<ArtifactAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ArtifactAuditPage> QueryAsync(ArtifactAuditQuery query, CancellationToken cancellationToken);
    ValueTask<ArtifactAuditMutation> SaveAsync(ArtifactAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IArtifactAuditEventSink { ValueTask PublishAsync(ArtifactAuditEvent domainEvent, CancellationToken cancellationToken); }