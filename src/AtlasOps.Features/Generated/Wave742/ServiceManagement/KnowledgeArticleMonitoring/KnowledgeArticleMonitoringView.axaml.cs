namespace AtlasOps.Features.ServiceManagement.KnowledgeArticleMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KnowledgeArticleMonitoringView : UserControl
{
    public KnowledgeArticleMonitoringView()
    {
        this.DataContext = new KnowledgeArticleMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KnowledgeArticleMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}