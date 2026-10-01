namespace AtlasOps.Features.Delivery.ReleaseRollbackRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseRollbackRecoveryView : UserControl
{
    public ReleaseRollbackRecoveryView()
    {
        this.DataContext = new ReleaseRollbackRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseRollbackRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}