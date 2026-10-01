namespace AtlasOps.Features.Cloud.AzureSubscriptionProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AzureSubscriptionProvisioningView : UserControl
{
    public AzureSubscriptionProvisioningView()
    {
        this.DataContext = new AzureSubscriptionProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AzureSubscriptionProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}