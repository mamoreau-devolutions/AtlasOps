namespace AtlasOps.Features.Observability.LogQueryProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class LogQueryProvisioningView : UserControl
{
    public LogQueryProvisioningView()
    {
        this.DataContext = new LogQueryProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is LogQueryProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}