namespace AtlasOps.Features.Mobile.MobileProfileGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileProfileGovernanceView : UserControl
{
    public MobileProfileGovernanceView()
    {
        this.DataContext = new MobileProfileGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileProfileGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}