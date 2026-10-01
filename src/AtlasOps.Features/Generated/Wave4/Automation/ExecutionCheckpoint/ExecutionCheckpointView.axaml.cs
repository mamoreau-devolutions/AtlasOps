namespace AtlasOps.Features.Automation.ExecutionCheckpoint;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ExecutionCheckpointView : UserControl
{
    public ExecutionCheckpointView()
    {
        this.DataContext = new ExecutionCheckpointViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ExecutionCheckpointViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}