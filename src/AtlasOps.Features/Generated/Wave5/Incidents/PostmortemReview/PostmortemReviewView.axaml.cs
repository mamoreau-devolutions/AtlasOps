namespace AtlasOps.Features.Incidents.PostmortemReview;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class PostmortemReviewView : UserControl
{
    public PostmortemReviewView()
    {
        this.DataContext = new PostmortemReviewViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is PostmortemReviewViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}