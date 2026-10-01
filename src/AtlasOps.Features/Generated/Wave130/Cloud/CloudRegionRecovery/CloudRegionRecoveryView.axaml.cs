namespace AtlasOps.Features.Cloud.CloudRegionRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudRegionRecoveryView : UserControl
{
    public CloudRegionRecoveryView()
    {
        this.DataContext = new CloudRegionRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudRegionRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}