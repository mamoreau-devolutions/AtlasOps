namespace AtlasOps.Features.Cloud.CloudStorageRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudStorageRecoveryView : UserControl
{
    public CloudStorageRecoveryView()
    {
        this.DataContext = new CloudStorageRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudStorageRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}