namespace AtlasOps.Features.Observability.ObservabilityRetentionRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObservabilityRetentionRecoveryView : UserControl
{
    public ObservabilityRetentionRecoveryView()
    {
        this.DataContext = new ObservabilityRetentionRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObservabilityRetentionRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}