namespace AtlasOps.Features.Architecture.ArchitectureReviewGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureReviewGovernanceView : UserControl
{
    public ArchitectureReviewGovernanceView()
    {
        this.DataContext = new ArchitectureReviewGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureReviewGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}