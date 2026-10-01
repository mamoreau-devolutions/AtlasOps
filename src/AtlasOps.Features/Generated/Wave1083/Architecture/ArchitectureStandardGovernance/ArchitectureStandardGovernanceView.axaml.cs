namespace AtlasOps.Features.Architecture.ArchitectureStandardGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureStandardGovernanceView : UserControl
{
    public ArchitectureStandardGovernanceView()
    {
        this.DataContext = new ArchitectureStandardGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureStandardGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}