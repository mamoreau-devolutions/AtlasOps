namespace AtlasOps.Features.Cloud.CloudDatabaseProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudDatabaseProvisioningView : UserControl
{
    public CloudDatabaseProvisioningView()
    {
        this.DataContext = new CloudDatabaseProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudDatabaseProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}