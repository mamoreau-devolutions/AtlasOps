namespace AtlasOps.Features.Cloud.GcpProjectGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class GcpProjectGovernanceView : UserControl
{
    public GcpProjectGovernanceView()
    {
        this.DataContext = new GcpProjectGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is GcpProjectGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}