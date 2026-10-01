namespace AtlasOps.Features.Cloud.AwsAccountProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AwsAccountProvisioningView : UserControl
{
    public AwsAccountProvisioningView()
    {
        this.DataContext = new AwsAccountProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AwsAccountProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}