namespace AtlasOps.Features.Platform.WorkspaceLifecycle;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class WorkspaceLifecycleView : UserControl
{
    public WorkspaceLifecycleView()
    {
        this.DataContext = new WorkspaceLifecycleViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is WorkspaceLifecycleViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}