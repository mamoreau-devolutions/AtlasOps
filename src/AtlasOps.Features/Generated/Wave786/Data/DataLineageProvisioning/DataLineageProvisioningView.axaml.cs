namespace AtlasOps.Features.Data.DataLineageProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataLineageProvisioningView : UserControl
{
    public DataLineageProvisioningView()
    {
        this.DataContext = new DataLineageProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataLineageProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}