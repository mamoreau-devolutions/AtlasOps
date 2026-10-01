namespace AtlasOps.Features.Platform.SettingsManagement;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SettingsManagementView : UserControl
{
    public SettingsManagementView()
    {
        this.DataContext = new SettingsManagementViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SettingsManagementViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}