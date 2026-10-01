namespace AtlasOps.Features.Sync.DurableOutbox;

using AtlasOps.Features;

public interface IDurableOutboxRepository : IAtlasOpsCapabilityRepository<DurableOutboxItem>;