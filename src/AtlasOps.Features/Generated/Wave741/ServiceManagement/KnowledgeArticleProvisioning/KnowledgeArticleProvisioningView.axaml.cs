namespace AtlasOps.Features.ServiceManagement.KnowledgeArticleProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KnowledgeArticleProvisioningView : UserControl
{
    public KnowledgeArticleProvisioningView()
    {
        this.DataContext = new KnowledgeArticleProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KnowledgeArticleProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}