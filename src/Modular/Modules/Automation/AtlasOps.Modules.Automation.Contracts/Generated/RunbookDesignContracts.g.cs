namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RunbookDesignState { Draft, Active, Paused, Completed, Archived }
public sealed record RunbookDesignRecord(Guid Id, string Name, string Owner, RunbookDesignState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RunbookDesignCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RunbookDesignEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RunbookDesignQuery(string? SearchText, RunbookDesignState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RunbookDesignPage(IReadOnlyList<RunbookDesignRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RunbookDesignMutation(bool Succeeded, string Code, string Message, RunbookDesignRecord? Record, RunbookDesignEvent? Event);
public interface IRunbookDesignRepository
{
    ValueTask<RunbookDesignRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RunbookDesignPage> QueryAsync(RunbookDesignQuery query, CancellationToken cancellationToken);
    ValueTask<RunbookDesignMutation> SaveAsync(RunbookDesignRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRunbookDesignEventSink { ValueTask PublishAsync(RunbookDesignEvent domainEvent, CancellationToken cancellationToken); }