namespace AtlasOps.Features.Observability.LogSourceProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class LogSourceProvisioningView : UserControl
{
    public LogSourceProvisioningView()
    {
        this.DataContext = new LogSourceProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is LogSourceProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}