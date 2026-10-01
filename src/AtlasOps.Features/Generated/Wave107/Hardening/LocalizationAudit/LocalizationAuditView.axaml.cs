namespace AtlasOps.Features.Hardening.LocalizationAudit;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class LocalizationAuditView : UserControl
{
    public LocalizationAuditView()
    {
        this.DataContext = new LocalizationAuditViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is LocalizationAuditViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}