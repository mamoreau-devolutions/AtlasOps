namespace AtlasOps.Features.Mobile.MobileSupportGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileSupportGovernanceView : UserControl
{
    public MobileSupportGovernanceView()
    {
        this.DataContext = new MobileSupportGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileSupportGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}