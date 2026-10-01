namespace AtlasOps.Features.Observability.ObservabilitySloRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObservabilitySloRecoveryView : UserControl
{
    public ObservabilitySloRecoveryView()
    {
        this.DataContext = new ObservabilitySloRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObservabilitySloRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}