namespace AtlasOps.Features.Api.ApiContractProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiContractProvisioningView : UserControl
{
    public ApiContractProvisioningView()
    {
        this.DataContext = new ApiContractProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiContractProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}