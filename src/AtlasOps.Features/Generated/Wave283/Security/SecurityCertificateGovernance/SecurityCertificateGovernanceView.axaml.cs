namespace AtlasOps.Features.Security.SecurityCertificateGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityCertificateGovernanceView : UserControl
{
    public SecurityCertificateGovernanceView()
    {
        this.DataContext = new SecurityCertificateGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityCertificateGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}