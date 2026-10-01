namespace AtlasOps.Features.Sync.BandwidthPolicy;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BandwidthPolicyView : UserControl
{
    public BandwidthPolicyView()
    {
        this.DataContext = new BandwidthPolicyViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BandwidthPolicyViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}