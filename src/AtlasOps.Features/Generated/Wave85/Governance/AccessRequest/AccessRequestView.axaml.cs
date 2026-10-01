namespace AtlasOps.Features.Governance.AccessRequest;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AccessRequestView : UserControl
{
    public AccessRequestView()
    {
        this.DataContext = new AccessRequestViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AccessRequestViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}