namespace AtlasOps.Modules.Automation.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum JobDesignState { Draft, Active, Paused, Completed, Archived }
public sealed record JobDesignRecord(Guid Id, string Name, string Owner, JobDesignState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record JobDesignCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record JobDesignEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record JobDesignQuery(string? SearchText, JobDesignState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record JobDesignPage(IReadOnlyList<JobDesignRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record JobDesignMutation(bool Succeeded, string Code, string Message, JobDesignRecord? Record, JobDesignEvent? Event);
public interface IJobDesignRepository
{
    ValueTask<JobDesignRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<JobDesignPage> QueryAsync(JobDesignQuery query, CancellationToken cancellationToken);
    ValueTask<JobDesignMutation> SaveAsync(JobDesignRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IJobDesignEventSink { ValueTask PublishAsync(JobDesignEvent domainEvent, CancellationToken cancellationToken); }