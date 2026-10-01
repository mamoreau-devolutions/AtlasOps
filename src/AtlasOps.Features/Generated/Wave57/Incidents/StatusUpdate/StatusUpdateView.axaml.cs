namespace AtlasOps.Features.Incidents.StatusUpdate;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StatusUpdateView : UserControl
{
    public StatusUpdateView()
    {
        this.DataContext = new StatusUpdateViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StatusUpdateViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}