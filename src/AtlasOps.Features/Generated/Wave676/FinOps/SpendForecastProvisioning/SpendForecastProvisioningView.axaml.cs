namespace AtlasOps.Features.FinOps.SpendForecastProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SpendForecastProvisioningView : UserControl
{
    public SpendForecastProvisioningView()
    {
        this.DataContext = new SpendForecastProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SpendForecastProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}