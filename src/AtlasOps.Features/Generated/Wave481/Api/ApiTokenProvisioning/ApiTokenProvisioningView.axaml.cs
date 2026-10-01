namespace AtlasOps.Features.Api.ApiTokenProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiTokenProvisioningView : UserControl
{
    public ApiTokenProvisioningView()
    {
        this.DataContext = new ApiTokenProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiTokenProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}