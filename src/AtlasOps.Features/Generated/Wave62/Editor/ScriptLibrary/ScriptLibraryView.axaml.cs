namespace AtlasOps.Features.Editor.ScriptLibrary;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ScriptLibraryView : UserControl
{
    public ScriptLibraryView()
    {
        this.DataContext = new ScriptLibraryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ScriptLibraryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}