namespace AtlasOps.Features.Incidents.ResponderAssignment;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ResponderAssignmentView : UserControl
{
    public ResponderAssignmentView()
    {
        this.DataContext = new ResponderAssignmentViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ResponderAssignmentViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}