namespace AtlasOps.Operations.Runtime;

using AtlasOps.Operations.Contracts;

public sealed class OperationLeaseManager
{
    private readonly object gate = new();
    private readonly Dictionary<Guid, OperationLease> leases = [];
    private long nextFencingToken;

    public LeaseAcquisitionResult TryAcquire(
        Guid jobId,
        string owner,
        DateTimeOffset now,
        TimeSpan duration)
    {
        if (jobId == Guid.Empty)
        {
            return new LeaseAcquisitionResult(false, "Job ID cannot be empty.", null);
        }

        if (string.IsNullOrWhiteSpace(owner))
        {
            return new LeaseAcquisitionResult(false, "Lease owner is required.", null);
        }

        if (duration < TimeSpan.FromSeconds(1) || duration > TimeSpan.FromHours(1))
        {
            return new LeaseAcquisitionResult(
                false,
                "Lease duration must be between one second and one hour.",
                null);
        }

        lock (this.gate)
        {
            if (this.leases.TryGetValue(jobId, out OperationLease? current) &&
                current.ExpiresAt > now)
            {
                return new LeaseAcquisitionResult(false, "The job has an active lease.", current);
            }

            OperationLease lease = new(
                jobId,
                owner,
                now,
                now.Add(duration),
                Interlocked.Increment(ref this.nextFencingToken));
            this.leases[jobId] = lease;
            return new LeaseAcquisitionResult(true, string.Empty, lease);
        }
    }

    public LeaseAcquisitionResult TryRenew(
        Guid jobId,
        string owner,
        long fencingToken,
        DateTimeOffset now,
        TimeSpan duration)
    {
        lock (this.gate)
        {
            if (!this.leases.TryGetValue(jobId, out OperationLease? lease))
            {
                return new LeaseAcquisitionResult(false, "The lease does not exist.", null);
            }

            if (!string.Equals(lease.Owner, owner, StringComparison.Ordinal) ||
                lease.FencingToken != fencingToken)
            {
                return new LeaseAcquisitionResult(false, "The lease owner or fencing token is stale.", lease);
            }

            if (lease.ExpiresAt <= now)
            {
                return new LeaseAcquisitionResult(false, "The lease has expired.", lease);
            }

            OperationLease renewed = lease with
            {
                ExpiresAt = now.Add(duration),
            };
            this.leases[jobId] = renewed;
            return new LeaseAcquisitionResult(true, string.Empty, renewed);
        }
    }

    public bool Release(Guid jobId, string owner, long fencingToken)
    {
        lock (this.gate)
        {
            if (!this.leases.TryGetValue(jobId, out OperationLease? lease) ||
                !string.Equals(lease.Owner, owner, StringComparison.Ordinal) ||
                lease.FencingToken != fencingToken)
            {
                return false;
            }

            return this.leases.Remove(jobId);
        }
    }
}
