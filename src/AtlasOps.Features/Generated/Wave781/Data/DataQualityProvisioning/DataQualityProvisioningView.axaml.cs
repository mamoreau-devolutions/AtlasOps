namespace AtlasOps.Features.Data.DataQualityProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataQualityProvisioningView : UserControl
{
    public DataQualityProvisioningView()
    {
        this.DataContext = new DataQualityProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataQualityProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}