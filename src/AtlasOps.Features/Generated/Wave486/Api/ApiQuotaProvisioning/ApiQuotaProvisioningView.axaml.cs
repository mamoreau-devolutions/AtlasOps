namespace AtlasOps.Features.Api.ApiQuotaProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiQuotaProvisioningView : UserControl
{
    public ApiQuotaProvisioningView()
    {
        this.DataContext = new ApiQuotaProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiQuotaProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}