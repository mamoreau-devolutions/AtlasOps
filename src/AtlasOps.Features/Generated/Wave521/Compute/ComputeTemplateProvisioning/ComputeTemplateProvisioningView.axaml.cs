namespace AtlasOps.Features.Compute.ComputeTemplateProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeTemplateProvisioningView : UserControl
{
    public ComputeTemplateProvisioningView()
    {
        this.DataContext = new ComputeTemplateProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeTemplateProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}