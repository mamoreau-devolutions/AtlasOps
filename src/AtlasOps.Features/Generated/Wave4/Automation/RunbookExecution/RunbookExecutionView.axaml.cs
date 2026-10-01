namespace AtlasOps.Features.Automation.RunbookExecution;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RunbookExecutionView : UserControl
{
    public RunbookExecutionView()
    {
        this.DataContext = new RunbookExecutionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RunbookExecutionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}