namespace AtlasOps.Features.ServiceManagement.ServiceScorecardProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceScorecardProvisioningView : UserControl
{
    public ServiceScorecardProvisioningView()
    {
        this.DataContext = new ServiceScorecardProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceScorecardProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}