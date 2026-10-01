namespace AtlasOps.Features.Governance.ComplianceControl;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComplianceControlView : UserControl
{
    public ComplianceControlView()
    {
        this.DataContext = new ComplianceControlViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComplianceControlViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}