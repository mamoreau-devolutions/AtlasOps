namespace AtlasOps.Features.Automation.DeploymentStage;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DeploymentStageView : UserControl
{
    public DeploymentStageView()
    {
        this.DataContext = new DeploymentStageViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DeploymentStageViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}