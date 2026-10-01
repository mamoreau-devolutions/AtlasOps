namespace AtlasOps.Features.Sync.BackupArchive;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BackupArchiveView : UserControl
{
    public BackupArchiveView()
    {
        this.DataContext = new BackupArchiveViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BackupArchiveViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}