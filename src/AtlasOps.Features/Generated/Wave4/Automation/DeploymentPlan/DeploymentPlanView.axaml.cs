namespace AtlasOps.Features.Automation.DeploymentPlan;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DeploymentPlanView : UserControl
{
    public DeploymentPlanView()
    {
        this.DataContext = new DeploymentPlanViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DeploymentPlanViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}