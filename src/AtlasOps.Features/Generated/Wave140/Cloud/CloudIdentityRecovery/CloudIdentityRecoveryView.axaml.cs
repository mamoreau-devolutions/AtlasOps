namespace AtlasOps.Features.Cloud.CloudIdentityRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudIdentityRecoveryView : UserControl
{
    public CloudIdentityRecoveryView()
    {
        this.DataContext = new CloudIdentityRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudIdentityRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}