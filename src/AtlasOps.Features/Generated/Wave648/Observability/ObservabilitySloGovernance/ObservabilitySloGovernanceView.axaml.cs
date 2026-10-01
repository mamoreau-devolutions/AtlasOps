namespace AtlasOps.Features.Observability.ObservabilitySloGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObservabilitySloGovernanceView : UserControl
{
    public ObservabilitySloGovernanceView()
    {
        this.DataContext = new ObservabilitySloGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObservabilitySloGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}