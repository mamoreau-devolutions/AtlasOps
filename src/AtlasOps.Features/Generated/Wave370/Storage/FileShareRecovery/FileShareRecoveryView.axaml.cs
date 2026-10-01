namespace AtlasOps.Features.Storage.FileShareRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class FileShareRecoveryView : UserControl
{
    public FileShareRecoveryView()
    {
        this.DataContext = new FileShareRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is FileShareRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}