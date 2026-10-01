namespace AtlasOps.Features.FinOps.SavingsPlanRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SavingsPlanRecoveryView : UserControl
{
    public SavingsPlanRecoveryView()
    {
        this.DataContext = new SavingsPlanRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SavingsPlanRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}