namespace AtlasOps.Features.Security.SecurityBaselineRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityBaselineRecoveryView : UserControl
{
    public SecurityBaselineRecoveryView()
    {
        this.DataContext = new SecurityBaselineRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityBaselineRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}