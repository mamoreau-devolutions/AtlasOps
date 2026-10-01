namespace AtlasOps.Features.Governance.ReviewCampaign;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReviewCampaignView : UserControl
{
    public ReviewCampaignView()
    {
        this.DataContext = new ReviewCampaignViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReviewCampaignViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}