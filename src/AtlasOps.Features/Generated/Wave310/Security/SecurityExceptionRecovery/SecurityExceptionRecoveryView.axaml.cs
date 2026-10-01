namespace AtlasOps.Features.Security.SecurityExceptionRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityExceptionRecoveryView : UserControl
{
    public SecurityExceptionRecoveryView()
    {
        this.DataContext = new SecurityExceptionRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityExceptionRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}