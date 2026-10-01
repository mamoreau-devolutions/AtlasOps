namespace AtlasOps.Features.Automation.WorkflowSimulation;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class WorkflowSimulationView : UserControl
{
    public WorkflowSimulationView()
    {
        this.DataContext = new WorkflowSimulationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is WorkflowSimulationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}