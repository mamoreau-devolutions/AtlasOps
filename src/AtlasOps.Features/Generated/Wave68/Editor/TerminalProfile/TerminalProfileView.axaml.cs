namespace AtlasOps.Features.Editor.TerminalProfile;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class TerminalProfileView : UserControl
{
    public TerminalProfileView()
    {
        this.DataContext = new TerminalProfileViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is TerminalProfileViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}