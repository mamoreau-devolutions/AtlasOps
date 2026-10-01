namespace AtlasOps.Integrations.ObjectStorage.Contracts;

public sealed record ObjectStorageEndpoint(
    string ProviderId,
    Uri ServiceUri,
    string Bucket,
    string CredentialReference);

public sealed record ObjectPart(
    int Number,
    long Offset,
    int Length,
    string Sha256);

public sealed record MultipartUploadPlan(
    string Bucket,
    string ObjectKey,
    long ContentLength,
    string ContentType,
    IReadOnlyList<ObjectPart> Parts,
    string ContentSha256);

public sealed record ObjectPlanValidation(
    bool Valid,
    IReadOnlyList<string> Diagnostics);
