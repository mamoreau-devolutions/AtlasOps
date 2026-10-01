namespace AtlasOps.Features.Cloud.CloudFunctionRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudFunctionRecoveryView : UserControl
{
    public CloudFunctionRecoveryView()
    {
        this.DataContext = new CloudFunctionRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudFunctionRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}