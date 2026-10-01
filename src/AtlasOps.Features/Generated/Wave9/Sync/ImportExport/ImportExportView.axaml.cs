namespace AtlasOps.Features.Sync.ImportExport;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ImportExportView : UserControl
{
    public ImportExportView()
    {
        this.DataContext = new ImportExportViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ImportExportViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}