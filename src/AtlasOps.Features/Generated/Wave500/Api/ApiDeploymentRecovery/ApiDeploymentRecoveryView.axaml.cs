namespace AtlasOps.Features.Api.ApiDeploymentRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiDeploymentRecoveryView : UserControl
{
    public ApiDeploymentRecoveryView()
    {
        this.DataContext = new ApiDeploymentRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiDeploymentRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}