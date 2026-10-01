namespace AtlasOps.Features.Automation.WorkflowVariable;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class WorkflowVariableView : UserControl
{
    public WorkflowVariableView()
    {
        this.DataContext = new WorkflowVariableViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is WorkflowVariableViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}