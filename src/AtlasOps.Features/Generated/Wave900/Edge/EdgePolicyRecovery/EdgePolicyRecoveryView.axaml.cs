namespace AtlasOps.Features.Edge.EdgePolicyRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgePolicyRecoveryView : UserControl
{
    public EdgePolicyRecoveryView()
    {
        this.DataContext = new EdgePolicyRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgePolicyRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}