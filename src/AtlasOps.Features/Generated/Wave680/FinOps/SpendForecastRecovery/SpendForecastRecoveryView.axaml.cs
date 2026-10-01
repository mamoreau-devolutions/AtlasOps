namespace AtlasOps.Features.FinOps.SpendForecastRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SpendForecastRecoveryView : UserControl
{
    public SpendForecastRecoveryView()
    {
        this.DataContext = new SpendForecastRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SpendForecastRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}