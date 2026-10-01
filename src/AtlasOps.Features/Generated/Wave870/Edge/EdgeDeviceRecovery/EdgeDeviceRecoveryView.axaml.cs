namespace AtlasOps.Features.Edge.EdgeDeviceRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeDeviceRecoveryView : UserControl
{
    public EdgeDeviceRecoveryView()
    {
        this.DataContext = new EdgeDeviceRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeDeviceRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}