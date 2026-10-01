namespace AtlasOps.Features.Automation.StepRetry;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StepRetryView : UserControl
{
    public StepRetryView()
    {
        this.DataContext = new StepRetryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StepRetryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}