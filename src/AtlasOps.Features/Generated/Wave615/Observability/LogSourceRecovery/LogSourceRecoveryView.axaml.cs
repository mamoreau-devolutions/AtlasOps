namespace AtlasOps.Features.Observability.LogSourceRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class LogSourceRecoveryView : UserControl
{
    public LogSourceRecoveryView()
    {
        this.DataContext = new LogSourceRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is LogSourceRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}