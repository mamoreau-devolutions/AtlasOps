namespace AtlasOps.Features.Automation.ArtifactPromotion;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArtifactPromotionView : UserControl
{
    public ArtifactPromotionView()
    {
        this.DataContext = new ArtifactPromotionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArtifactPromotionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}