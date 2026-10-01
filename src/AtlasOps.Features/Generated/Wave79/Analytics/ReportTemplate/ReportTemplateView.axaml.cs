namespace AtlasOps.Features.Analytics.ReportTemplate;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReportTemplateView : UserControl
{
    public ReportTemplateView()
    {
        this.DataContext = new ReportTemplateViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReportTemplateViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}