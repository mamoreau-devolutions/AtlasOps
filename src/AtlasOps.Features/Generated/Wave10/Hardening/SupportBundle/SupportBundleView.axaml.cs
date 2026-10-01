namespace AtlasOps.Features.Hardening.SupportBundle;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SupportBundleView : UserControl
{
    public SupportBundleView()
    {
        this.DataContext = new SupportBundleViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SupportBundleViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}