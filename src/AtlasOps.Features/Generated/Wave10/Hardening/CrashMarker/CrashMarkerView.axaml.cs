namespace AtlasOps.Features.Hardening.CrashMarker;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CrashMarkerView : UserControl
{
    public CrashMarkerView()
    {
        this.DataContext = new CrashMarkerViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CrashMarkerViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}