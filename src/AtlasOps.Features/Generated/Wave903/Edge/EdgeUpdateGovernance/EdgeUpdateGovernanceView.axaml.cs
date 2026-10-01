namespace AtlasOps.Features.Edge.EdgeUpdateGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeUpdateGovernanceView : UserControl
{
    public EdgeUpdateGovernanceView()
    {
        this.DataContext = new EdgeUpdateGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeUpdateGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}