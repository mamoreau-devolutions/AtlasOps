namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum MilestoneArchivalState { Draft, Active, Paused, Completed, Archived }
public sealed record MilestoneArchivalRecord(Guid Id, string Name, string Owner, MilestoneArchivalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record MilestoneArchivalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record MilestoneArchivalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record MilestoneArchivalQuery(string? SearchText, MilestoneArchivalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record MilestoneArchivalPage(IReadOnlyList<MilestoneArchivalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record MilestoneArchivalMutation(bool Succeeded, string Code, string Message, MilestoneArchivalRecord? Record, MilestoneArchivalEvent? Event);
public interface IMilestoneArchivalRepository
{
    ValueTask<MilestoneArchivalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<MilestoneArchivalPage> QueryAsync(MilestoneArchivalQuery query, CancellationToken cancellationToken);
    ValueTask<MilestoneArchivalMutation> SaveAsync(MilestoneArchivalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IMilestoneArchivalEventSink { ValueTask PublishAsync(MilestoneArchivalEvent domainEvent, CancellationToken cancellationToken); }