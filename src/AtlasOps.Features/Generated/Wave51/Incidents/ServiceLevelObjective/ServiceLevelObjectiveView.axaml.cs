namespace AtlasOps.Features.Incidents.ServiceLevelObjective;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceLevelObjectiveView : UserControl
{
    public ServiceLevelObjectiveView()
    {
        this.DataContext = new ServiceLevelObjectiveViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceLevelObjectiveViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}