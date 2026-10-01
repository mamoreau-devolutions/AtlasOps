namespace AtlasOps.Features.Observability.ObservabilityExportRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObservabilityExportRecoveryView : UserControl
{
    public ObservabilityExportRecoveryView()
    {
        this.DataContext = new ObservabilityExportRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObservabilityExportRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}