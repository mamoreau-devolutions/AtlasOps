namespace AtlasOps.Features.Observability.ObservabilityRetentionProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObservabilityRetentionProvisioningView : UserControl
{
    public ObservabilityRetentionProvisioningView()
    {
        this.DataContext = new ObservabilityRetentionProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObservabilityRetentionProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}