namespace AtlasOps.Modules.Credentials.Core;

using System.Collections.Generic;

public sealed record CredentialsCapabilityDescriptor(string Id, string DisplayName, string Area, string Concern, string Description);
public static class CredentialsModule
{
    public const string Id = "Credentials";
    public const string DisplayName = "Credentials and secrets";
    public static IReadOnlyList<CredentialsCapabilityDescriptor> Capabilities { get; } = new CredentialsCapabilityDescriptor[]
    {
        new("Credentials.CredentialLifecycle", "Credential lifecycle", "Credential", "Lifecycle", "Coordinates credentials and secrets for Credential lifecycle."),
        new("Credentials.CredentialAccess", "Credential access", "Credential", "Access", "Coordinates credentials and secrets for Credential access."),
        new("Credentials.CredentialRotation", "Credential rotation", "Credential", "Rotation", "Coordinates credentials and secrets for Credential rotation."),
        new("Credentials.CredentialVerification", "Credential verification", "Credential", "Verification", "Coordinates credentials and secrets for Credential verification."),
        new("Credentials.CredentialRecovery", "Credential recovery", "Credential", "Recovery", "Coordinates credentials and secrets for Credential recovery."),
        new("Credentials.CredentialAudit", "Credential audit", "Credential", "Audit", "Coordinates credentials and secrets for Credential audit."),
        new("Credentials.SecretLifecycle", "Secret lifecycle", "Secret", "Lifecycle", "Coordinates credentials and secrets for Secret lifecycle."),
        new("Credentials.SecretAccess", "Secret access", "Secret", "Access", "Coordinates credentials and secrets for Secret access."),
        new("Credentials.SecretRotation", "Secret rotation", "Secret", "Rotation", "Coordinates credentials and secrets for Secret rotation."),
        new("Credentials.SecretVerification", "Secret verification", "Secret", "Verification", "Coordinates credentials and secrets for Secret verification."),
        new("Credentials.SecretRecovery", "Secret recovery", "Secret", "Recovery", "Coordinates credentials and secrets for Secret recovery."),
        new("Credentials.SecretAudit", "Secret audit", "Secret", "Audit", "Coordinates credentials and secrets for Secret audit."),
        new("Credentials.VaultLifecycle", "Vault lifecycle", "Vault", "Lifecycle", "Coordinates credentials and secrets for Vault lifecycle."),
        new("Credentials.VaultAccess", "Vault access", "Vault", "Access", "Coordinates credentials and secrets for Vault access."),
        new("Credentials.VaultRotation", "Vault rotation", "Vault", "Rotation", "Coordinates credentials and secrets for Vault rotation."),
        new("Credentials.VaultVerification", "Vault verification", "Vault", "Verification", "Coordinates credentials and secrets for Vault verification."),
        new("Credentials.VaultRecovery", "Vault recovery", "Vault", "Recovery", "Coordinates credentials and secrets for Vault recovery."),
        new("Credentials.VaultAudit", "Vault audit", "Vault", "Audit", "Coordinates credentials and secrets for Vault audit."),
        new("Credentials.ResolverLifecycle", "Resolver lifecycle", "Resolver", "Lifecycle", "Coordinates credentials and secrets for Resolver lifecycle."),
        new("Credentials.ResolverAccess", "Resolver access", "Resolver", "Access", "Coordinates credentials and secrets for Resolver access."),
        new("Credentials.ResolverRotation", "Resolver rotation", "Resolver", "Rotation", "Coordinates credentials and secrets for Resolver rotation."),
        new("Credentials.ResolverVerification", "Resolver verification", "Resolver", "Verification", "Coordinates credentials and secrets for Resolver verification."),
        new("Credentials.ResolverRecovery", "Resolver recovery", "Resolver", "Recovery", "Coordinates credentials and secrets for Resolver recovery."),
        new("Credentials.ResolverAudit", "Resolver audit", "Resolver", "Audit", "Coordinates credentials and secrets for Resolver audit."),
        new("Credentials.RotationLifecycle", "Rotation lifecycle", "Rotation", "Lifecycle", "Coordinates credentials and secrets for Rotation lifecycle."),
        new("Credentials.RotationAccess", "Rotation access", "Rotation", "Access", "Coordinates credentials and secrets for Rotation access."),
        new("Credentials.RotationRotation", "Rotation rotation", "Rotation", "Rotation", "Coordinates credentials and secrets for Rotation rotation."),
        new("Credentials.RotationVerification", "Rotation verification", "Rotation", "Verification", "Coordinates credentials and secrets for Rotation verification."),
        new("Credentials.RotationRecovery", "Rotation recovery", "Rotation", "Recovery", "Coordinates credentials and secrets for Rotation recovery."),
        new("Credentials.RotationAudit", "Rotation audit", "Rotation", "Audit", "Coordinates credentials and secrets for Rotation audit."),
    };
}