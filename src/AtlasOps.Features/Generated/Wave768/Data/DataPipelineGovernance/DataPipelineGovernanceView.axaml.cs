namespace AtlasOps.Features.Data.DataPipelineGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataPipelineGovernanceView : UserControl
{
    public DataPipelineGovernanceView()
    {
        this.DataContext = new DataPipelineGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataPipelineGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}