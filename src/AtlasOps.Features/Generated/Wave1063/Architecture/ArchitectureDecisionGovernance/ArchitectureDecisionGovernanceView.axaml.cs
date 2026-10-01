namespace AtlasOps.Features.Architecture.ArchitectureDecisionGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureDecisionGovernanceView : UserControl
{
    public ArchitectureDecisionGovernanceView()
    {
        this.DataContext = new ArchitectureDecisionGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureDecisionGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}