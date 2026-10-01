namespace AtlasOps.Features.Governance.PolicySimulation;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class PolicySimulationView : UserControl
{
    public PolicySimulationView()
    {
        this.DataContext = new PolicySimulationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is PolicySimulationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}