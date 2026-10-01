namespace AtlasOps.Adapters.Aws;

using Amazon.EC2;
using Amazon.IdentityManagement;
using Amazon.Route53;
using Amazon.S3;
using Amazon.SSO;
using Amazon.SSOOIDC;

public sealed record AwsSdkServiceDescriptor(string Id, string DisplayName, Type ClientType, string RequiredPermission);

public static class AwsSdkCatalog
{
    public static IReadOnlyList<AwsSdkServiceDescriptor> Services { get; } =
    [
        new("ec2", "Elastic Compute Cloud", typeof(AmazonEC2Client), "ec2:DescribeInstances"),
        new("iam", "Identity and Access Management", typeof(AmazonIdentityManagementServiceClient), "iam:ListRoles"),
        new("route53", "Route 53", typeof(AmazonRoute53Client), "route53:ListHostedZones"),
        new("s3", "Simple Storage Service", typeof(AmazonS3Client), "s3:ListAllMyBuckets"),
        new("sso", "IAM Identity Center", typeof(AmazonSSOClient), "sso:ListAccountRoles"),
        new("sso-oidc", "IAM Identity Center OIDC", typeof(AmazonSSOOIDCClient), "sso-oauth:CreateToken"),
    ];
}
