namespace AtlasOps.Features.Sync.DisasterRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DisasterRecoveryView : UserControl
{
    public DisasterRecoveryView()
    {
        this.DataContext = new DisasterRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DisasterRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}