namespace AtlasOps.Features.Cloud.CloudIdentityOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudIdentityOptimizationView : UserControl
{
    public CloudIdentityOptimizationView()
    {
        this.DataContext = new CloudIdentityOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudIdentityOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}