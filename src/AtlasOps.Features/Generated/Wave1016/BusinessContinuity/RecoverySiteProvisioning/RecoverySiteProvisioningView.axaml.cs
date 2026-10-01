namespace AtlasOps.Features.BusinessContinuity.RecoverySiteProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoverySiteProvisioningView : UserControl
{
    public RecoverySiteProvisioningView()
    {
        this.DataContext = new RecoverySiteProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoverySiteProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}