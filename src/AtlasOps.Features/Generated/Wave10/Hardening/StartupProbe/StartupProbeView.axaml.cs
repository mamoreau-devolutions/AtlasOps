namespace AtlasOps.Features.Hardening.StartupProbe;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StartupProbeView : UserControl
{
    public StartupProbeView()
    {
        this.DataContext = new StartupProbeViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StartupProbeViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}