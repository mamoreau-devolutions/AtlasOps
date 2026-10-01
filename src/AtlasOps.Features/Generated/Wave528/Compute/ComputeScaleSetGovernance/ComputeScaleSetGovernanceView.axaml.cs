namespace AtlasOps.Features.Compute.ComputeScaleSetGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeScaleSetGovernanceView : UserControl
{
    public ComputeScaleSetGovernanceView()
    {
        this.DataContext = new ComputeScaleSetGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeScaleSetGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}