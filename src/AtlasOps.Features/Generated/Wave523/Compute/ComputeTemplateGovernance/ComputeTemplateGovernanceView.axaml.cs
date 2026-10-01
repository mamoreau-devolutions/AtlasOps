namespace AtlasOps.Features.Compute.ComputeTemplateGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeTemplateGovernanceView : UserControl
{
    public ComputeTemplateGovernanceView()
    {
        this.DataContext = new ComputeTemplateGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeTemplateGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}