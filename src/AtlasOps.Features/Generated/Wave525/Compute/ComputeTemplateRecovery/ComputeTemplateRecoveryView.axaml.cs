namespace AtlasOps.Features.Compute.ComputeTemplateRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeTemplateRecoveryView : UserControl
{
    public ComputeTemplateRecoveryView()
    {
        this.DataContext = new ComputeTemplateRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeTemplateRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}