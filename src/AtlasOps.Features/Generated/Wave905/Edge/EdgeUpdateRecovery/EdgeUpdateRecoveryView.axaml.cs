namespace AtlasOps.Features.Edge.EdgeUpdateRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeUpdateRecoveryView : UserControl
{
    public EdgeUpdateRecoveryView()
    {
        this.DataContext = new EdgeUpdateRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeUpdateRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}