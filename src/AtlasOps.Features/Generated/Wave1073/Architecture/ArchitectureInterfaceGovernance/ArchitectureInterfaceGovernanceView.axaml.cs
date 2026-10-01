namespace AtlasOps.Features.Architecture.ArchitectureInterfaceGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureInterfaceGovernanceView : UserControl
{
    public ArchitectureInterfaceGovernanceView()
    {
        this.DataContext = new ArchitectureInterfaceGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureInterfaceGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}