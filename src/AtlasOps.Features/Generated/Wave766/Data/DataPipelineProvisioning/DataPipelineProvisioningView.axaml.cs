namespace AtlasOps.Features.Data.DataPipelineProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataPipelineProvisioningView : UserControl
{
    public DataPipelineProvisioningView()
    {
        this.DataContext = new DataPipelineProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataPipelineProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}