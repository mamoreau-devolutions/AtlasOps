namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PortValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record PortValidationRecord(Guid Id, string Name, string Owner, PortValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PortValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PortValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PortValidationQuery(string? SearchText, PortValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PortValidationPage(IReadOnlyList<PortValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PortValidationMutation(bool Succeeded, string Code, string Message, PortValidationRecord? Record, PortValidationEvent? Event);
public interface IPortValidationRepository
{
    ValueTask<PortValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PortValidationPage> QueryAsync(PortValidationQuery query, CancellationToken cancellationToken);
    ValueTask<PortValidationMutation> SaveAsync(PortValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPortValidationEventSink { ValueTask PublishAsync(PortValidationEvent domainEvent, CancellationToken cancellationToken); }