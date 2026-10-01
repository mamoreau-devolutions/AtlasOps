namespace AtlasOps.Features.Network.NetworkAddressProvisioning;

using AtlasOps.Features;

public sealed class NetworkAddressProvisioningItem : IAtlasOpsCapabilityEntity
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N");

    public string Name { get; set; } = "Network Address Provisioning";

    public string Owner { get; set; } = "Operations";

    public string State { get; set; } = "Draft";

    public int Priority { get; set; } = 6;

    public bool IsEnabled { get; set; } = true;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public int Revision { get; private set; }

    public void MarkUpdated(DateTimeOffset timestamp)
    {
        this.UpdatedAt = timestamp;
        this.Revision++;
    }
}