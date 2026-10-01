namespace AtlasOps.Features.Api.ApiClientRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiClientRecoveryView : UserControl
{
    public ApiClientRecoveryView()
    {
        this.DataContext = new ApiClientRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiClientRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}