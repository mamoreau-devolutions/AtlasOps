namespace AtlasOps.Features.Storage.ObjectBucketOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObjectBucketOptimizationView : UserControl
{
    public ObjectBucketOptimizationView()
    {
        this.DataContext = new ObjectBucketOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObjectBucketOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}