namespace AtlasOps.Features.Data.DataContractProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataContractProvisioningView : UserControl
{
    public DataContractProvisioningView()
    {
        this.DataContext = new DataContractProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataContractProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}