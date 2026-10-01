namespace AtlasOps.Features.Architecture.ArchitectureComponentGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureComponentGovernanceView : UserControl
{
    public ArchitectureComponentGovernanceView()
    {
        this.DataContext = new ArchitectureComponentGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureComponentGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}