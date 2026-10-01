namespace AtlasOps.Features.Analytics.ReportDefinition;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReportDefinitionView : UserControl
{
    public ReportDefinitionView()
    {
        this.DataContext = new ReportDefinitionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReportDefinitionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}