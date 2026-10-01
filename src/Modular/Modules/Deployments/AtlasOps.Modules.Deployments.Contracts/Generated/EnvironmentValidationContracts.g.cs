namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EnvironmentValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record EnvironmentValidationRecord(Guid Id, string Name, string Owner, EnvironmentValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EnvironmentValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EnvironmentValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EnvironmentValidationQuery(string? SearchText, EnvironmentValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EnvironmentValidationPage(IReadOnlyList<EnvironmentValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EnvironmentValidationMutation(bool Succeeded, string Code, string Message, EnvironmentValidationRecord? Record, EnvironmentValidationEvent? Event);
public interface IEnvironmentValidationRepository
{
    ValueTask<EnvironmentValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EnvironmentValidationPage> QueryAsync(EnvironmentValidationQuery query, CancellationToken cancellationToken);
    ValueTask<EnvironmentValidationMutation> SaveAsync(EnvironmentValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEnvironmentValidationEventSink { ValueTask PublishAsync(EnvironmentValidationEvent domainEvent, CancellationToken cancellationToken); }