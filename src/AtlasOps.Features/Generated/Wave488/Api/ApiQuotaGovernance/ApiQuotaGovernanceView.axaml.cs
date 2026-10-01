namespace AtlasOps.Features.Api.ApiQuotaGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiQuotaGovernanceView : UserControl
{
    public ApiQuotaGovernanceView()
    {
        this.DataContext = new ApiQuotaGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiQuotaGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}