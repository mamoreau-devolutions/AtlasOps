namespace AtlasOps.Features.Edge.EdgeDeploymentRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeDeploymentRecoveryView : UserControl
{
    public EdgeDeploymentRecoveryView()
    {
        this.DataContext = new EdgeDeploymentRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeDeploymentRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}