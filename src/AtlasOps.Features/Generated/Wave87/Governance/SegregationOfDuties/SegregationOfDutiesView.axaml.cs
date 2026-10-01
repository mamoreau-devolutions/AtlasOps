namespace AtlasOps.Features.Governance.SegregationOfDuties;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SegregationOfDutiesView : UserControl
{
    public SegregationOfDutiesView()
    {
        this.DataContext = new SegregationOfDutiesViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SegregationOfDutiesViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}