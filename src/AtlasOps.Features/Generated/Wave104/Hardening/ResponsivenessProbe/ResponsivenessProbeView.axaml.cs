namespace AtlasOps.Features.Hardening.ResponsivenessProbe;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ResponsivenessProbeView : UserControl
{
    public ResponsivenessProbeView()
    {
        this.DataContext = new ResponsivenessProbeViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ResponsivenessProbeViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}