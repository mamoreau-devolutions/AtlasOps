namespace AtlasOps.Features.Automation.CanaryRollout;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CanaryRolloutView : UserControl
{
    public CanaryRolloutView()
    {
        this.DataContext = new CanaryRolloutViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CanaryRolloutViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}