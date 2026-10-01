namespace AtlasOps.Features.Api.ApiAnalyticsProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiAnalyticsProvisioningView : UserControl
{
    public ApiAnalyticsProvisioningView()
    {
        this.DataContext = new ApiAnalyticsProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiAnalyticsProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}