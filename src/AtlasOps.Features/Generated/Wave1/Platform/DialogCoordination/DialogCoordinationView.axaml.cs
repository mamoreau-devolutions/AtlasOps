namespace AtlasOps.Features.Platform.DialogCoordination;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DialogCoordinationView : UserControl
{
    public DialogCoordinationView()
    {
        this.DataContext = new DialogCoordinationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DialogCoordinationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}