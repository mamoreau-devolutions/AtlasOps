namespace AtlasOps.Features.Architecture.ArchitectureRiskGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureRiskGovernanceView : UserControl
{
    public ArchitectureRiskGovernanceView()
    {
        this.DataContext = new ArchitectureRiskGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureRiskGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}