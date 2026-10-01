namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EndpointAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record EndpointAuditRecord(Guid Id, string Name, string Owner, EndpointAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EndpointAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EndpointAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EndpointAuditQuery(string? SearchText, EndpointAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EndpointAuditPage(IReadOnlyList<EndpointAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EndpointAuditMutation(bool Succeeded, string Code, string Message, EndpointAuditRecord? Record, EndpointAuditEvent? Event);
public interface IEndpointAuditRepository
{
    ValueTask<EndpointAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EndpointAuditPage> QueryAsync(EndpointAuditQuery query, CancellationToken cancellationToken);
    ValueTask<EndpointAuditMutation> SaveAsync(EndpointAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEndpointAuditEventSink { ValueTask PublishAsync(EndpointAuditEvent domainEvent, CancellationToken cancellationToken); }