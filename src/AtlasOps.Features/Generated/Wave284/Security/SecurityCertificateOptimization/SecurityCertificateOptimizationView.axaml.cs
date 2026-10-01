namespace AtlasOps.Features.Security.SecurityCertificateOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityCertificateOptimizationView : UserControl
{
    public SecurityCertificateOptimizationView()
    {
        this.DataContext = new SecurityCertificateOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityCertificateOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}