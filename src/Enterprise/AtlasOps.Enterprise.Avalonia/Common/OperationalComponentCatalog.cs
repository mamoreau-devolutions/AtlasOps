namespace AtlasOps.Enterprise.Avalonia.Common;

using global::Avalonia.Controls;

using AvaloniaEdit;

public sealed record OperationalComponentDescriptor(string Id, Type ControlType, string PackageId, string Purpose);

public static class OperationalComponentCatalog
{
    public static IReadOnlyList<OperationalComponentDescriptor> Components { get; } =
    [
        new("data-grid", typeof(DataGrid), "Avalonia.Controls.DataGrid", "Tabular operations and result sets"),
        new("code-editor", typeof(TextEditor), "Avalonia.AvaloniaEdit", "Workflow, policy, and query editing"),
        new("svg", typeof(Control), "Svg.Controls.Avalonia", "Scalable status and topology graphics"),
        new("qr", typeof(Image), "QRCoder", "Enrollment and handoff codes"),
        new("barcode", typeof(Image), "ZXing.Net", "Asset label decoding"),
    ];
}
