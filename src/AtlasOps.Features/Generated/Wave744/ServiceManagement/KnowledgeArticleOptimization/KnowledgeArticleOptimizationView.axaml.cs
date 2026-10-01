namespace AtlasOps.Features.ServiceManagement.KnowledgeArticleOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KnowledgeArticleOptimizationView : UserControl
{
    public KnowledgeArticleOptimizationView()
    {
        this.DataContext = new KnowledgeArticleOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KnowledgeArticleOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}