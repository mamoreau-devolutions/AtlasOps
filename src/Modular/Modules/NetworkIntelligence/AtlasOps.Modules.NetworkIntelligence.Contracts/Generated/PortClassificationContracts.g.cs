namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PortClassificationState { Draft, Active, Paused, Completed, Archived }
public sealed record PortClassificationRecord(Guid Id, string Name, string Owner, PortClassificationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PortClassificationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PortClassificationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PortClassificationQuery(string? SearchText, PortClassificationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PortClassificationPage(IReadOnlyList<PortClassificationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PortClassificationMutation(bool Succeeded, string Code, string Message, PortClassificationRecord? Record, PortClassificationEvent? Event);
public interface IPortClassificationRepository
{
    ValueTask<PortClassificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PortClassificationPage> QueryAsync(PortClassificationQuery query, CancellationToken cancellationToken);
    ValueTask<PortClassificationMutation> SaveAsync(PortClassificationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPortClassificationEventSink { ValueTask PublishAsync(PortClassificationEvent domainEvent, CancellationToken cancellationToken); }