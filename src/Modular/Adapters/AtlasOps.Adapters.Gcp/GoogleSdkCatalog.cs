namespace AtlasOps.Adapters.Gcp;

using Google.Apis.Auth.OAuth2;

public sealed record GoogleSdkServiceDescriptor(string Id, Type CredentialType, IReadOnlyList<string> Scopes);

public static class GoogleSdkCatalog
{
    public static GoogleSdkServiceDescriptor Default { get; } = new(
        "google-cloud",
        typeof(GoogleCredential),
        [
            "https://www.googleapis.com/auth/cloud-platform.read-only",
            "https://www.googleapis.com/auth/compute.readonly",
        ]);
}
