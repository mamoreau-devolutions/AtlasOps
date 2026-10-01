namespace AtlasOps.Features.Edge.EdgeApplicationRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeApplicationRecoveryView : UserControl
{
    public EdgeApplicationRecoveryView()
    {
        this.DataContext = new EdgeApplicationRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeApplicationRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}