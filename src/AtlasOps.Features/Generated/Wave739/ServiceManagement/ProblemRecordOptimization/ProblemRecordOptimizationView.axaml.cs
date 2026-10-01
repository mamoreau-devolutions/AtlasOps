namespace AtlasOps.Features.ServiceManagement.ProblemRecordOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ProblemRecordOptimizationView : UserControl
{
    public ProblemRecordOptimizationView()
    {
        this.DataContext = new ProblemRecordOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ProblemRecordOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}