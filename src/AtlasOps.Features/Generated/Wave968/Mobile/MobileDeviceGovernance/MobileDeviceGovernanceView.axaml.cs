namespace AtlasOps.Features.Mobile.MobileDeviceGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileDeviceGovernanceView : UserControl
{
    public MobileDeviceGovernanceView()
    {
        this.DataContext = new MobileDeviceGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileDeviceGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}