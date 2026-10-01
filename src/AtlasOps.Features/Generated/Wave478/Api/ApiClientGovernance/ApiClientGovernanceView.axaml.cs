namespace AtlasOps.Features.Api.ApiClientGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiClientGovernanceView : UserControl
{
    public ApiClientGovernanceView()
    {
        this.DataContext = new ApiClientGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiClientGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}