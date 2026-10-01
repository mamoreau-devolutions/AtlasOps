namespace AtlasOps.Features.Editor.DataExport;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataExportView : UserControl
{
    public DataExportView()
    {
        this.DataContext = new DataExportViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataExportViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}