namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RotationAccessState { Draft, Active, Paused, Completed, Archived }
public sealed record RotationAccessRecord(Guid Id, string Name, string Owner, RotationAccessState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RotationAccessCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RotationAccessEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RotationAccessQuery(string? SearchText, RotationAccessState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RotationAccessPage(IReadOnlyList<RotationAccessRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RotationAccessMutation(bool Succeeded, string Code, string Message, RotationAccessRecord? Record, RotationAccessEvent? Event);
public interface IRotationAccessRepository
{
    ValueTask<RotationAccessRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RotationAccessPage> QueryAsync(RotationAccessQuery query, CancellationToken cancellationToken);
    ValueTask<RotationAccessMutation> SaveAsync(RotationAccessRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRotationAccessEventSink { ValueTask PublishAsync(RotationAccessEvent domainEvent, CancellationToken cancellationToken); }