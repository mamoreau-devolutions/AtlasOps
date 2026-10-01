namespace AtlasOps.Features.Sync.WorkspaceClone;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class WorkspaceCloneView : UserControl
{
    public WorkspaceCloneView()
    {
        this.DataContext = new WorkspaceCloneViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is WorkspaceCloneViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}