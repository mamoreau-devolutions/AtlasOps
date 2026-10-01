namespace AtlasOps.Features.Delivery.ReleaseApprovalRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseApprovalRecoveryView : UserControl
{
    public ReleaseApprovalRecoveryView()
    {
        this.DataContext = new ReleaseApprovalRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseApprovalRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}