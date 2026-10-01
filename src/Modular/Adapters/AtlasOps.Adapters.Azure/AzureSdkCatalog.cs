namespace AtlasOps.Adapters.Azure;

using global::Azure.Identity;
using global::Azure.ResourceManager.Authorization;
using global::Azure.Security.KeyVault.Secrets;
using global::Azure.Storage.Blobs;

public sealed record AzureSdkServiceDescriptor(string Id, string DisplayName, Type ClientType, string Scope);

public static class AzureSdkCatalog
{
    public static IReadOnlyList<AzureSdkServiceDescriptor> Services { get; } =
    [
        new("identity", "Azure Identity", typeof(DefaultAzureCredential), "https://management.azure.com/.default"),
        new("authorization", "Azure Authorization", typeof(RoleAssignmentResource), "Microsoft.Authorization/roleAssignments/read"),
        new("blob", "Azure Blob Storage", typeof(BlobServiceClient), "https://storage.azure.com/.default"),
        new("key-vault", "Azure Key Vault", typeof(SecretClient), "https://vault.azure.net/.default"),
    ];
}
