namespace AtlasOps.Features.Hardening.UpgradeChannel;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class UpgradeChannelView : UserControl
{
    public UpgradeChannelView()
    {
        this.DataContext = new UpgradeChannelViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is UpgradeChannelViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}