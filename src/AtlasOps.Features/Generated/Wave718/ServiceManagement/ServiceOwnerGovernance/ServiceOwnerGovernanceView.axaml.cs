namespace AtlasOps.Features.ServiceManagement.ServiceOwnerGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceOwnerGovernanceView : UserControl
{
    public ServiceOwnerGovernanceView()
    {
        this.DataContext = new ServiceOwnerGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceOwnerGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}