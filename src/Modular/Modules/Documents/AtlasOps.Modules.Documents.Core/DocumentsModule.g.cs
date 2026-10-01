namespace AtlasOps.Modules.Documents.Core;

using System.Collections.Generic;

public sealed record DocumentsCapabilityDescriptor(string Id, string DisplayName, string Area, string Concern, string Description);
public static class DocumentsModule
{
    public const string Id = "Documents";
    public const string DisplayName = "Documents and knowledge";
    public static IReadOnlyList<DocumentsCapabilityDescriptor> Capabilities { get; } = new DocumentsCapabilityDescriptor[]
    {
        new("Documents.DocumentAuthoring", "Document authoring", "Document", "Authoring", "Coordinates documents and knowledge for Document authoring."),
        new("Documents.DocumentIndexing", "Document indexing", "Document", "Indexing", "Coordinates documents and knowledge for Document indexing."),
        new("Documents.DocumentVersioning", "Document versioning", "Document", "Versioning", "Coordinates documents and knowledge for Document versioning."),
        new("Documents.DocumentApproval", "Document approval", "Document", "Approval", "Coordinates documents and knowledge for Document approval."),
        new("Documents.DocumentPublishing", "Document publishing", "Document", "Publishing", "Coordinates documents and knowledge for Document publishing."),
        new("Documents.DocumentArchival", "Document archival", "Document", "Archival", "Coordinates documents and knowledge for Document archival."),
        new("Documents.ArticleAuthoring", "Article authoring", "Article", "Authoring", "Coordinates documents and knowledge for Article authoring."),
        new("Documents.ArticleIndexing", "Article indexing", "Article", "Indexing", "Coordinates documents and knowledge for Article indexing."),
        new("Documents.ArticleVersioning", "Article versioning", "Article", "Versioning", "Coordinates documents and knowledge for Article versioning."),
        new("Documents.ArticleApproval", "Article approval", "Article", "Approval", "Coordinates documents and knowledge for Article approval."),
        new("Documents.ArticlePublishing", "Article publishing", "Article", "Publishing", "Coordinates documents and knowledge for Article publishing."),
        new("Documents.ArticleArchival", "Article archival", "Article", "Archival", "Coordinates documents and knowledge for Article archival."),
        new("Documents.TemplateAuthoring", "Template authoring", "Template", "Authoring", "Coordinates documents and knowledge for Template authoring."),
        new("Documents.TemplateIndexing", "Template indexing", "Template", "Indexing", "Coordinates documents and knowledge for Template indexing."),
        new("Documents.TemplateVersioning", "Template versioning", "Template", "Versioning", "Coordinates documents and knowledge for Template versioning."),
        new("Documents.TemplateApproval", "Template approval", "Template", "Approval", "Coordinates documents and knowledge for Template approval."),
        new("Documents.TemplatePublishing", "Template publishing", "Template", "Publishing", "Coordinates documents and knowledge for Template publishing."),
        new("Documents.TemplateArchival", "Template archival", "Template", "Archival", "Coordinates documents and knowledge for Template archival."),
        new("Documents.RevisionAuthoring", "Revision authoring", "Revision", "Authoring", "Coordinates documents and knowledge for Revision authoring."),
        new("Documents.RevisionIndexing", "Revision indexing", "Revision", "Indexing", "Coordinates documents and knowledge for Revision indexing."),
        new("Documents.RevisionVersioning", "Revision versioning", "Revision", "Versioning", "Coordinates documents and knowledge for Revision versioning."),
        new("Documents.RevisionApproval", "Revision approval", "Revision", "Approval", "Coordinates documents and knowledge for Revision approval."),
        new("Documents.RevisionPublishing", "Revision publishing", "Revision", "Publishing", "Coordinates documents and knowledge for Revision publishing."),
        new("Documents.RevisionArchival", "Revision archival", "Revision", "Archival", "Coordinates documents and knowledge for Revision archival."),
        new("Documents.LinkAuthoring", "Link authoring", "Link", "Authoring", "Coordinates documents and knowledge for Link authoring."),
        new("Documents.LinkIndexing", "Link indexing", "Link", "Indexing", "Coordinates documents and knowledge for Link indexing."),
        new("Documents.LinkVersioning", "Link versioning", "Link", "Versioning", "Coordinates documents and knowledge for Link versioning."),
        new("Documents.LinkApproval", "Link approval", "Link", "Approval", "Coordinates documents and knowledge for Link approval."),
        new("Documents.LinkPublishing", "Link publishing", "Link", "Publishing", "Coordinates documents and knowledge for Link publishing."),
        new("Documents.LinkArchival", "Link archival", "Link", "Archival", "Coordinates documents and knowledge for Link archival."),
    };
}