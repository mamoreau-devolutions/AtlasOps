namespace AtlasOps.Features.Mobile.MobileApplicationGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileApplicationGovernanceView : UserControl
{
    public MobileApplicationGovernanceView()
    {
        this.DataContext = new MobileApplicationGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileApplicationGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}