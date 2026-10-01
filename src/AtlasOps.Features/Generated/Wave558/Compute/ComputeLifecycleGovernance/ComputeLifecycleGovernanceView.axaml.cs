namespace AtlasOps.Features.Compute.ComputeLifecycleGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeLifecycleGovernanceView : UserControl
{
    public ComputeLifecycleGovernanceView()
    {
        this.DataContext = new ComputeLifecycleGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeLifecycleGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}