namespace AtlasOps.Features.Data.DataDatasetProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataDatasetProvisioningView : UserControl
{
    public DataDatasetProvisioningView()
    {
        this.DataContext = new DataDatasetProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataDatasetProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}