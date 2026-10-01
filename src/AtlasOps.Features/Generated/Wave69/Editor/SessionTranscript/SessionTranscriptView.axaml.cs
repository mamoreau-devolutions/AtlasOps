namespace AtlasOps.Features.Editor.SessionTranscript;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SessionTranscriptView : UserControl
{
    public SessionTranscriptView()
    {
        this.DataContext = new SessionTranscriptViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SessionTranscriptViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}