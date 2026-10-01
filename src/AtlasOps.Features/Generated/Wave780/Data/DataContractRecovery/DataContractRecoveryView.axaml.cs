namespace AtlasOps.Features.Data.DataContractRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataContractRecoveryView : UserControl
{
    public DataContractRecoveryView()
    {
        this.DataContext = new DataContractRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataContractRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}