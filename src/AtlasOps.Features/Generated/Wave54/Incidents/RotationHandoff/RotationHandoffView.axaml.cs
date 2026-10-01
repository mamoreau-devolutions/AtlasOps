namespace AtlasOps.Features.Incidents.RotationHandoff;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RotationHandoffView : UserControl
{
    public RotationHandoffView()
    {
        this.DataContext = new RotationHandoffViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RotationHandoffViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}