namespace AtlasOps.Features.Storage.FileShareOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class FileShareOptimizationView : UserControl
{
    public FileShareOptimizationView()
    {
        this.DataContext = new FileShareOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is FileShareOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}