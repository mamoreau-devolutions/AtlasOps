namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProtocolValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record ProtocolValidationRecord(Guid Id, string Name, string Owner, ProtocolValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProtocolValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProtocolValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProtocolValidationQuery(string? SearchText, ProtocolValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProtocolValidationPage(IReadOnlyList<ProtocolValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProtocolValidationMutation(bool Succeeded, string Code, string Message, ProtocolValidationRecord? Record, ProtocolValidationEvent? Event);
public interface IProtocolValidationRepository
{
    ValueTask<ProtocolValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProtocolValidationPage> QueryAsync(ProtocolValidationQuery query, CancellationToken cancellationToken);
    ValueTask<ProtocolValidationMutation> SaveAsync(ProtocolValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProtocolValidationEventSink { ValueTask PublishAsync(ProtocolValidationEvent domainEvent, CancellationToken cancellationToken); }