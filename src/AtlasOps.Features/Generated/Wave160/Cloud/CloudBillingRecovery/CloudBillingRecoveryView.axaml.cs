namespace AtlasOps.Features.Cloud.CloudBillingRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudBillingRecoveryView : UserControl
{
    public CloudBillingRecoveryView()
    {
        this.DataContext = new CloudBillingRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudBillingRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}