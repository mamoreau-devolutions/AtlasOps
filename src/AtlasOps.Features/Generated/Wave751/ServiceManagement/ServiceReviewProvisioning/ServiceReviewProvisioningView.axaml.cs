namespace AtlasOps.Features.ServiceManagement.ServiceReviewProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceReviewProvisioningView : UserControl
{
    public ServiceReviewProvisioningView()
    {
        this.DataContext = new ServiceReviewProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceReviewProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}