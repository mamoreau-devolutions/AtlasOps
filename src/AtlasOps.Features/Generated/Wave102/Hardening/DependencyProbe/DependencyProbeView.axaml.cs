namespace AtlasOps.Features.Hardening.DependencyProbe;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DependencyProbeView : UserControl
{
    public DependencyProbeView()
    {
        this.DataContext = new DependencyProbeViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DependencyProbeViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}