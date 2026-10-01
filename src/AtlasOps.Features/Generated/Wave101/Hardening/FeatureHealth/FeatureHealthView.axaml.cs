namespace AtlasOps.Features.Hardening.FeatureHealth;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class FeatureHealthView : UserControl
{
    public FeatureHealthView()
    {
        this.DataContext = new FeatureHealthViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is FeatureHealthViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}