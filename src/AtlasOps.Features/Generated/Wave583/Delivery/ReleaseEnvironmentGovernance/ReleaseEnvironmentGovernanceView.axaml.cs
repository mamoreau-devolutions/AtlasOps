namespace AtlasOps.Features.Delivery.ReleaseEnvironmentGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseEnvironmentGovernanceView : UserControl
{
    public ReleaseEnvironmentGovernanceView()
    {
        this.DataContext = new ReleaseEnvironmentGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseEnvironmentGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}