namespace AtlasOps.Features.Governance.AuditExport;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AuditExportView : UserControl
{
    public AuditExportView()
    {
        this.DataContext = new AuditExportViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AuditExportViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}