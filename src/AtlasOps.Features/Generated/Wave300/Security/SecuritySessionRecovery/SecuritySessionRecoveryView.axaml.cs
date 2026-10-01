namespace AtlasOps.Features.Security.SecuritySessionRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecuritySessionRecoveryView : UserControl
{
    public SecuritySessionRecoveryView()
    {
        this.DataContext = new SecuritySessionRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecuritySessionRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}