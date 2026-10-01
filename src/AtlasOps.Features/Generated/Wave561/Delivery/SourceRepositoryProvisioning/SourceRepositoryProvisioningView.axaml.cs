namespace AtlasOps.Features.Delivery.SourceRepositoryProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SourceRepositoryProvisioningView : UserControl
{
    public SourceRepositoryProvisioningView()
    {
        this.DataContext = new SourceRepositoryProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SourceRepositoryProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}