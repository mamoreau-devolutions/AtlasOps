namespace AtlasOps.Features.Automation.DeploymentFreeze;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DeploymentFreezeView : UserControl
{
    public DeploymentFreezeView()
    {
        this.DataContext = new DeploymentFreezeViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DeploymentFreezeViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}