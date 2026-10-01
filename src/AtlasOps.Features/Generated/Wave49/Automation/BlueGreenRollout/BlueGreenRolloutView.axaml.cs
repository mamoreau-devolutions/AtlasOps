namespace AtlasOps.Features.Automation.BlueGreenRollout;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BlueGreenRolloutView : UserControl
{
    public BlueGreenRolloutView()
    {
        this.DataContext = new BlueGreenRolloutViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BlueGreenRolloutViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}