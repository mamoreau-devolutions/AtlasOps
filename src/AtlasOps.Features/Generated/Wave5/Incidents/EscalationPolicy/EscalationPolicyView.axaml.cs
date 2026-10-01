namespace AtlasOps.Features.Incidents.EscalationPolicy;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EscalationPolicyView : UserControl
{
    public EscalationPolicyView()
    {
        this.DataContext = new EscalationPolicyViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EscalationPolicyViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}