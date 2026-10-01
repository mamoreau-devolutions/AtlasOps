namespace AtlasOps.Features.Architecture.ArchitectureExceptionGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureExceptionGovernanceView : UserControl
{
    public ArchitectureExceptionGovernanceView()
    {
        this.DataContext = new ArchitectureExceptionGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureExceptionGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}