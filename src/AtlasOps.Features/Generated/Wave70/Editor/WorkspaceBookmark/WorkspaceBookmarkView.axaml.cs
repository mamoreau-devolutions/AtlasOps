namespace AtlasOps.Features.Editor.WorkspaceBookmark;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class WorkspaceBookmarkView : UserControl
{
    public WorkspaceBookmarkView()
    {
        this.DataContext = new WorkspaceBookmarkViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is WorkspaceBookmarkViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}