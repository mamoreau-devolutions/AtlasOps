namespace AtlasOps.Features.Api.ApiVersionRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiVersionRecoveryView : UserControl
{
    public ApiVersionRecoveryView()
    {
        this.DataContext = new ApiVersionRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiVersionRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}