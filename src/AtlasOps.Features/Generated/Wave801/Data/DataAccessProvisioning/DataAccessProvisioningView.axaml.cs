namespace AtlasOps.Features.Data.DataAccessProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataAccessProvisioningView : UserControl
{
    public DataAccessProvisioningView()
    {
        this.DataContext = new DataAccessProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataAccessProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}