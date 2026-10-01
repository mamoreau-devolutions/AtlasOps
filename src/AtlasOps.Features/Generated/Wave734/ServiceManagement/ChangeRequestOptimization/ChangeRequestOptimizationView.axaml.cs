namespace AtlasOps.Features.ServiceManagement.ChangeRequestOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ChangeRequestOptimizationView : UserControl
{
    public ChangeRequestOptimizationView()
    {
        this.DataContext = new ChangeRequestOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ChangeRequestOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}