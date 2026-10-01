namespace AtlasOps.Features.Observability.TraceSourceRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class TraceSourceRecoveryView : UserControl
{
    public TraceSourceRecoveryView()
    {
        this.DataContext = new TraceSourceRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is TraceSourceRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}