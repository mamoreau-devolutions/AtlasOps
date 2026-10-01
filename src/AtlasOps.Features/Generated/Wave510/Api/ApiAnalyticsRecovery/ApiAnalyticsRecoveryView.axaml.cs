namespace AtlasOps.Features.Api.ApiAnalyticsRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiAnalyticsRecoveryView : UserControl
{
    public ApiAnalyticsRecoveryView()
    {
        this.DataContext = new ApiAnalyticsRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiAnalyticsRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}