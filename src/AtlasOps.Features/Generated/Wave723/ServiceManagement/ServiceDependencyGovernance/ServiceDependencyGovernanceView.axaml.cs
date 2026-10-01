namespace AtlasOps.Features.ServiceManagement.ServiceDependencyGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceDependencyGovernanceView : UserControl
{
    public ServiceDependencyGovernanceView()
    {
        this.DataContext = new ServiceDependencyGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceDependencyGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}