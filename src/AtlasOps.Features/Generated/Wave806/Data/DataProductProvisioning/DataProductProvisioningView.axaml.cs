namespace AtlasOps.Features.Data.DataProductProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataProductProvisioningView : UserControl
{
    public DataProductProvisioningView()
    {
        this.DataContext = new DataProductProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataProductProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}