namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProtocolClassificationState { Draft, Active, Paused, Completed, Archived }
public sealed record ProtocolClassificationRecord(Guid Id, string Name, string Owner, ProtocolClassificationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProtocolClassificationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProtocolClassificationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProtocolClassificationQuery(string? SearchText, ProtocolClassificationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProtocolClassificationPage(IReadOnlyList<ProtocolClassificationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProtocolClassificationMutation(bool Succeeded, string Code, string Message, ProtocolClassificationRecord? Record, ProtocolClassificationEvent? Event);
public interface IProtocolClassificationRepository
{
    ValueTask<ProtocolClassificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProtocolClassificationPage> QueryAsync(ProtocolClassificationQuery query, CancellationToken cancellationToken);
    ValueTask<ProtocolClassificationMutation> SaveAsync(ProtocolClassificationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProtocolClassificationEventSink { ValueTask PublishAsync(ProtocolClassificationEvent domainEvent, CancellationToken cancellationToken); }