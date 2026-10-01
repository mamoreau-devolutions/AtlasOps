namespace AtlasOps.Features.Delivery.ReleaseGateRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseGateRecoveryView : UserControl
{
    public ReleaseGateRecoveryView()
    {
        this.DataContext = new ReleaseGateRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseGateRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}