namespace AtlasOps.Features.ServiceManagement.ChangeRequestProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ChangeRequestProvisioningView : UserControl
{
    public ChangeRequestProvisioningView()
    {
        this.DataContext = new ChangeRequestProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ChangeRequestProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}