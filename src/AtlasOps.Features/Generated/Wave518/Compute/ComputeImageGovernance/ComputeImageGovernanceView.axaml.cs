namespace AtlasOps.Features.Compute.ComputeImageGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeImageGovernanceView : UserControl
{
    public ComputeImageGovernanceView()
    {
        this.DataContext = new ComputeImageGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeImageGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}