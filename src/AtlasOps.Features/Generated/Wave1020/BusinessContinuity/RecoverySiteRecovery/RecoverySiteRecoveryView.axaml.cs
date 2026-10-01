namespace AtlasOps.Features.BusinessContinuity.RecoverySiteRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoverySiteRecoveryView : UserControl
{
    public RecoverySiteRecoveryView()
    {
        this.DataContext = new RecoverySiteRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoverySiteRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}