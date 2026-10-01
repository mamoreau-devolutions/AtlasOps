namespace AtlasOps.Features.Connections.SessionRecording;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SessionRecordingView : UserControl
{
    public SessionRecordingView()
    {
        this.DataContext = new SessionRecordingViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SessionRecordingViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}