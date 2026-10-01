namespace AtlasOps.Features.Platform.ClockAbstraction;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ClockAbstractionView : UserControl
{
    public ClockAbstractionView()
    {
        this.DataContext = new ClockAbstractionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ClockAbstractionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}