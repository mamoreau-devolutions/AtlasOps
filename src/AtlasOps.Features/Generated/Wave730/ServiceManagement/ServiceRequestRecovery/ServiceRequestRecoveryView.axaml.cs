namespace AtlasOps.Features.ServiceManagement.ServiceRequestRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceRequestRecoveryView : UserControl
{
    public ServiceRequestRecoveryView()
    {
        this.DataContext = new ServiceRequestRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceRequestRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}