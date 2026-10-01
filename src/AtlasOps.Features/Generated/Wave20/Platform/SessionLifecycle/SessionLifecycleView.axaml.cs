namespace AtlasOps.Features.Platform.SessionLifecycle;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SessionLifecycleView : UserControl
{
    public SessionLifecycleView()
    {
        this.DataContext = new SessionLifecycleViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SessionLifecycleViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}