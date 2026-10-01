namespace AtlasOps.Features.Api.ApiVersionProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiVersionProvisioningView : UserControl
{
    public ApiVersionProvisioningView()
    {
        this.DataContext = new ApiVersionProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiVersionProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}