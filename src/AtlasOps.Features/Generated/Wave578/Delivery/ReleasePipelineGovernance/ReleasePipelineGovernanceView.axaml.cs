namespace AtlasOps.Features.Delivery.ReleasePipelineGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleasePipelineGovernanceView : UserControl
{
    public ReleasePipelineGovernanceView()
    {
        this.DataContext = new ReleasePipelineGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleasePipelineGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}