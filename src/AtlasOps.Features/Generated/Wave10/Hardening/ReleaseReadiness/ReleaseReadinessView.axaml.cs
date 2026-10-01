namespace AtlasOps.Features.Hardening.ReleaseReadiness;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseReadinessView : UserControl
{
    public ReleaseReadinessView()
    {
        this.DataContext = new ReleaseReadinessViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseReadinessViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}