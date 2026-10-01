namespace AtlasOps.Features.Hardening.AccessibilityAudit;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AccessibilityAuditView : UserControl
{
    public AccessibilityAuditView()
    {
        this.DataContext = new AccessibilityAuditViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AccessibilityAuditViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}