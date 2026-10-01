namespace AtlasOps.Features.Architecture.ArchitectureEvidenceGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureEvidenceGovernanceView : UserControl
{
    public ArchitectureEvidenceGovernanceView()
    {
        this.DataContext = new ArchitectureEvidenceGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureEvidenceGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}