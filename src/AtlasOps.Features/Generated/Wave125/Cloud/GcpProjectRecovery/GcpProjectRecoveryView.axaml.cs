namespace AtlasOps.Features.Cloud.GcpProjectRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class GcpProjectRecoveryView : UserControl
{
    public GcpProjectRecoveryView()
    {
        this.DataContext = new GcpProjectRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is GcpProjectRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}