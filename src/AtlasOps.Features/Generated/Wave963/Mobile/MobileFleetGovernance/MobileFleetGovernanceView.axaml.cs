namespace AtlasOps.Features.Mobile.MobileFleetGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileFleetGovernanceView : UserControl
{
    public MobileFleetGovernanceView()
    {
        this.DataContext = new MobileFleetGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileFleetGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}