namespace AtlasOps.Features.Automation.ManualGate;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ManualGateView : UserControl
{
    public ManualGateView()
    {
        this.DataContext = new ManualGateViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ManualGateViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}