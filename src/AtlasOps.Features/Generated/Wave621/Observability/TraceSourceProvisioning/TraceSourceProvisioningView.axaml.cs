namespace AtlasOps.Features.Observability.TraceSourceProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class TraceSourceProvisioningView : UserControl
{
    public TraceSourceProvisioningView()
    {
        this.DataContext = new TraceSourceProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is TraceSourceProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}