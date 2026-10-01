namespace AtlasOps.Features.Cloud.AwsAccountOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AwsAccountOptimizationView : UserControl
{
    public AwsAccountOptimizationView()
    {
        this.DataContext = new AwsAccountOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AwsAccountOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}