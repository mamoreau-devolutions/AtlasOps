namespace AtlasOps.Features.Data.DataTransformProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataTransformProvisioningView : UserControl
{
    public DataTransformProvisioningView()
    {
        this.DataContext = new DataTransformProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataTransformProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}