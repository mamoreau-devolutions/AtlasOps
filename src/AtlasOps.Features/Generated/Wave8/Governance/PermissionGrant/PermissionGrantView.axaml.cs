namespace AtlasOps.Features.Governance.PermissionGrant;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class PermissionGrantView : UserControl
{
    public PermissionGrantView()
    {
        this.DataContext = new PermissionGrantViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is PermissionGrantViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}