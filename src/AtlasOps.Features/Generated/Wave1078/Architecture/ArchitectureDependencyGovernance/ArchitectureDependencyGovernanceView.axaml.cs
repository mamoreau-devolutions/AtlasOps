namespace AtlasOps.Features.Architecture.ArchitectureDependencyGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureDependencyGovernanceView : UserControl
{
    public ArchitectureDependencyGovernanceView()
    {
        this.DataContext = new ArchitectureDependencyGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureDependencyGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}