namespace AtlasOps.Features.Data.DataContractGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataContractGovernanceView : UserControl
{
    public DataContractGovernanceView()
    {
        this.DataContext = new DataContractGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataContractGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}