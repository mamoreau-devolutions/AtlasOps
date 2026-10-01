namespace AtlasOps.Features.Data.DataRetentionOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataRetentionOptimizationView : UserControl
{
    public DataRetentionOptimizationView()
    {
        this.DataContext = new DataRetentionOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataRetentionOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}