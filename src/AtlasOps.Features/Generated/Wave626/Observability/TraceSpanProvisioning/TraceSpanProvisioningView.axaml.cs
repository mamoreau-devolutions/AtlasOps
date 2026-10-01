namespace AtlasOps.Features.Observability.TraceSpanProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class TraceSpanProvisioningView : UserControl
{
    public TraceSpanProvisioningView()
    {
        this.DataContext = new TraceSpanProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is TraceSpanProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}