namespace AtlasOps.Features.Api.ApiHealthProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiHealthProvisioningView : UserControl
{
    public ApiHealthProvisioningView()
    {
        this.DataContext = new ApiHealthProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiHealthProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}