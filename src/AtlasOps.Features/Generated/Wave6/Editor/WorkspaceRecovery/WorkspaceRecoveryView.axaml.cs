namespace AtlasOps.Features.Editor.WorkspaceRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class WorkspaceRecoveryView : UserControl
{
    public WorkspaceRecoveryView()
    {
        this.DataContext = new WorkspaceRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is WorkspaceRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}