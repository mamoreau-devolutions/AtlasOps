namespace AtlasOps.Features.ServiceManagement.ServiceOwnerRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceOwnerRecoveryView : UserControl
{
    public ServiceOwnerRecoveryView()
    {
        this.DataContext = new ServiceOwnerRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceOwnerRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}