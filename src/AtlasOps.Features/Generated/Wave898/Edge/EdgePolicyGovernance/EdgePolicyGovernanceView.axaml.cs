namespace AtlasOps.Features.Edge.EdgePolicyGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgePolicyGovernanceView : UserControl
{
    public EdgePolicyGovernanceView()
    {
        this.DataContext = new EdgePolicyGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgePolicyGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}