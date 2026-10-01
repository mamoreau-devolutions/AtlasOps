namespace AtlasOps.Features.FinOps.ResourceCommitmentGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ResourceCommitmentGovernanceView : UserControl
{
    public ResourceCommitmentGovernanceView()
    {
        this.DataContext = new ResourceCommitmentGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ResourceCommitmentGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}