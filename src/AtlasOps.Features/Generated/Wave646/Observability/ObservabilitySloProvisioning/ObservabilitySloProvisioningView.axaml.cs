namespace AtlasOps.Features.Observability.ObservabilitySloProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObservabilitySloProvisioningView : UserControl
{
    public ObservabilitySloProvisioningView()
    {
        this.DataContext = new ObservabilitySloProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObservabilitySloProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}