namespace AtlasOps.Features.Delivery.ReleaseEnvironmentRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseEnvironmentRecoveryView : UserControl
{
    public ReleaseEnvironmentRecoveryView()
    {
        this.DataContext = new ReleaseEnvironmentRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseEnvironmentRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}