namespace AtlasOps.Features.Automation.ParallelStage;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ParallelStageView : UserControl
{
    public ParallelStageView()
    {
        this.DataContext = new ParallelStageViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ParallelStageViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}