namespace AtlasOps.Features.Automation.ChangeWindow;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ChangeWindowView : UserControl
{
    public ChangeWindowView()
    {
        this.DataContext = new ChangeWindowViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ChangeWindowViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}