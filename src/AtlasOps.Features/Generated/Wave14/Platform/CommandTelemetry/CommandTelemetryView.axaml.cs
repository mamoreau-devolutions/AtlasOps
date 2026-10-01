namespace AtlasOps.Features.Platform.CommandTelemetry;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CommandTelemetryView : UserControl
{
    public CommandTelemetryView()
    {
        this.DataContext = new CommandTelemetryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CommandTelemetryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}