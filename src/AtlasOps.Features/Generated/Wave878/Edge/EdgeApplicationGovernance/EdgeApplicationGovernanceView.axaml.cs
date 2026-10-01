namespace AtlasOps.Features.Edge.EdgeApplicationGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeApplicationGovernanceView : UserControl
{
    public EdgeApplicationGovernanceView()
    {
        this.DataContext = new EdgeApplicationGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeApplicationGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}