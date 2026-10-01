namespace AtlasOps.Features.Api.ApiDeploymentGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiDeploymentGovernanceView : UserControl
{
    public ApiDeploymentGovernanceView()
    {
        this.DataContext = new ApiDeploymentGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiDeploymentGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}