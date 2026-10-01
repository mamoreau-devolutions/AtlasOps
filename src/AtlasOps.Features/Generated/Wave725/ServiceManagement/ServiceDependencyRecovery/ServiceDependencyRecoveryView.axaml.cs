namespace AtlasOps.Features.ServiceManagement.ServiceDependencyRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceDependencyRecoveryView : UserControl
{
    public ServiceDependencyRecoveryView()
    {
        this.DataContext = new ServiceDependencyRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceDependencyRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}