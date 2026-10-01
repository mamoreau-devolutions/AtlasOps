namespace AtlasOps.Features.Mobile.MobileCertificateOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileCertificateOptimizationView : UserControl
{
    public MobileCertificateOptimizationView()
    {
        this.DataContext = new MobileCertificateOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileCertificateOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}