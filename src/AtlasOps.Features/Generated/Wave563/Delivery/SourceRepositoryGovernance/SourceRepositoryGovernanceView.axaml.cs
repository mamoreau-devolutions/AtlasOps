namespace AtlasOps.Features.Delivery.SourceRepositoryGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SourceRepositoryGovernanceView : UserControl
{
    public SourceRepositoryGovernanceView()
    {
        this.DataContext = new SourceRepositoryGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SourceRepositoryGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}