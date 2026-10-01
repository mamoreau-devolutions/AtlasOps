namespace AtlasOps.Features.Api.ApiContractGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiContractGovernanceView : UserControl
{
    public ApiContractGovernanceView()
    {
        this.DataContext = new ApiContractGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiContractGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}