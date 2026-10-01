namespace AtlasOps.Features.Security.SecurityFindingRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityFindingRecoveryView : UserControl
{
    public SecurityFindingRecoveryView()
    {
        this.DataContext = new SecurityFindingRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityFindingRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}