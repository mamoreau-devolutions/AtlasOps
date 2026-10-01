namespace AtlasOps.Features.Platform.FeatureToggle;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class FeatureToggleView : UserControl
{
    public FeatureToggleView()
    {
        this.DataContext = new FeatureToggleViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is FeatureToggleViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}