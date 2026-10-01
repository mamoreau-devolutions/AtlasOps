namespace AtlasOps.Modules.NetworkIntelligence.Core;

using System.Collections.Generic;

public sealed record NetworkIntelligenceCapabilityDescriptor(string Id, string DisplayName, string Area, string Concern, string Description);
public static class NetworkIntelligenceModule
{
    public const string Id = "NetworkIntelligence";
    public const string DisplayName = "IANA network intelligence";
    public static IReadOnlyList<NetworkIntelligenceCapabilityDescriptor> Capabilities { get; } = new NetworkIntelligenceCapabilityDescriptor[]
    {
        new("NetworkIntelligence.ServiceCatalog", "Service catalog", "Service", "Catalog", "Coordinates iana network intelligence for Service catalog."),
        new("NetworkIntelligence.ServiceClassification", "Service classification", "Service", "Classification", "Coordinates iana network intelligence for Service classification."),
        new("NetworkIntelligence.ServiceValidation", "Service validation", "Service", "Validation", "Coordinates iana network intelligence for Service validation."),
        new("NetworkIntelligence.ServicePolicy", "Service policy", "Service", "Policy", "Coordinates iana network intelligence for Service policy."),
        new("NetworkIntelligence.ServiceCollision", "Service collision", "Service", "Collision", "Coordinates iana network intelligence for Service collision."),
        new("NetworkIntelligence.ServiceReporting", "Service reporting", "Service", "Reporting", "Coordinates iana network intelligence for Service reporting."),
        new("NetworkIntelligence.PortCatalog", "Port catalog", "Port", "Catalog", "Coordinates iana network intelligence for Port catalog."),
        new("NetworkIntelligence.PortClassification", "Port classification", "Port", "Classification", "Coordinates iana network intelligence for Port classification."),
        new("NetworkIntelligence.PortValidation", "Port validation", "Port", "Validation", "Coordinates iana network intelligence for Port validation."),
        new("NetworkIntelligence.PortPolicy", "Port policy", "Port", "Policy", "Coordinates iana network intelligence for Port policy."),
        new("NetworkIntelligence.PortCollision", "Port collision", "Port", "Collision", "Coordinates iana network intelligence for Port collision."),
        new("NetworkIntelligence.PortReporting", "Port reporting", "Port", "Reporting", "Coordinates iana network intelligence for Port reporting."),
        new("NetworkIntelligence.ProtocolCatalog", "Protocol catalog", "Protocol", "Catalog", "Coordinates iana network intelligence for Protocol catalog."),
        new("NetworkIntelligence.ProtocolClassification", "Protocol classification", "Protocol", "Classification", "Coordinates iana network intelligence for Protocol classification."),
        new("NetworkIntelligence.ProtocolValidation", "Protocol validation", "Protocol", "Validation", "Coordinates iana network intelligence for Protocol validation."),
        new("NetworkIntelligence.ProtocolPolicy", "Protocol policy", "Protocol", "Policy", "Coordinates iana network intelligence for Protocol policy."),
        new("NetworkIntelligence.ProtocolCollision", "Protocol collision", "Protocol", "Collision", "Coordinates iana network intelligence for Protocol collision."),
        new("NetworkIntelligence.ProtocolReporting", "Protocol reporting", "Protocol", "Reporting", "Coordinates iana network intelligence for Protocol reporting."),
        new("NetworkIntelligence.CipherCatalog", "Cipher catalog", "Cipher", "Catalog", "Coordinates iana network intelligence for Cipher catalog."),
        new("NetworkIntelligence.CipherClassification", "Cipher classification", "Cipher", "Classification", "Coordinates iana network intelligence for Cipher classification."),
        new("NetworkIntelligence.CipherValidation", "Cipher validation", "Cipher", "Validation", "Coordinates iana network intelligence for Cipher validation."),
        new("NetworkIntelligence.CipherPolicy", "Cipher policy", "Cipher", "Policy", "Coordinates iana network intelligence for Cipher policy."),
        new("NetworkIntelligence.CipherCollision", "Cipher collision", "Cipher", "Collision", "Coordinates iana network intelligence for Cipher collision."),
        new("NetworkIntelligence.CipherReporting", "Cipher reporting", "Cipher", "Reporting", "Coordinates iana network intelligence for Cipher reporting."),
        new("NetworkIntelligence.AssignmentCatalog", "Assignment catalog", "Assignment", "Catalog", "Coordinates iana network intelligence for Assignment catalog."),
        new("NetworkIntelligence.AssignmentClassification", "Assignment classification", "Assignment", "Classification", "Coordinates iana network intelligence for Assignment classification."),
        new("NetworkIntelligence.AssignmentValidation", "Assignment validation", "Assignment", "Validation", "Coordinates iana network intelligence for Assignment validation."),
        new("NetworkIntelligence.AssignmentPolicy", "Assignment policy", "Assignment", "Policy", "Coordinates iana network intelligence for Assignment policy."),
        new("NetworkIntelligence.AssignmentCollision", "Assignment collision", "Assignment", "Collision", "Coordinates iana network intelligence for Assignment collision."),
        new("NetworkIntelligence.AssignmentReporting", "Assignment reporting", "Assignment", "Reporting", "Coordinates iana network intelligence for Assignment reporting."),
    };
}