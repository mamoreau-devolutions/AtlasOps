namespace AtlasOps.Features.Governance.AuditEnvelope;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AuditEnvelopeView : UserControl
{
    public AuditEnvelopeView()
    {
        this.DataContext = new AuditEnvelopeViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AuditEnvelopeViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}