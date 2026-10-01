namespace AtlasOps.Features.Observability.LogQueryRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class LogQueryRecoveryView : UserControl
{
    public LogQueryRecoveryView()
    {
        this.DataContext = new LogQueryRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is LogQueryRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}