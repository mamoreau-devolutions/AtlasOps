namespace AtlasOps.Features.Compute.ComputePatchGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputePatchGovernanceView : UserControl
{
    public ComputePatchGovernanceView()
    {
        this.DataContext = new ComputePatchGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputePatchGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}