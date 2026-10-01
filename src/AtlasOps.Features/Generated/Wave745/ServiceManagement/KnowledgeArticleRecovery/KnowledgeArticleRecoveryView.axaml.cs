namespace AtlasOps.Features.ServiceManagement.KnowledgeArticleRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KnowledgeArticleRecoveryView : UserControl
{
    public KnowledgeArticleRecoveryView()
    {
        this.DataContext = new KnowledgeArticleRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KnowledgeArticleRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}