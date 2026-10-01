namespace AtlasOps.Features.Security.SecurityKeyRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityKeyRecoveryView : UserControl
{
    public SecurityKeyRecoveryView()
    {
        this.DataContext = new SecurityKeyRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityKeyRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}