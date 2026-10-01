namespace AtlasOps.Features.Sync.DataArchive;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataArchiveView : UserControl
{
    public DataArchiveView()
    {
        this.DataContext = new DataArchiveViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataArchiveViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}