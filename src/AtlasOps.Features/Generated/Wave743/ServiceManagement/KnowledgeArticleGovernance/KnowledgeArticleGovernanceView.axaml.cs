namespace AtlasOps.Features.ServiceManagement.KnowledgeArticleGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KnowledgeArticleGovernanceView : UserControl
{
    public KnowledgeArticleGovernanceView()
    {
        this.DataContext = new KnowledgeArticleGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KnowledgeArticleGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}