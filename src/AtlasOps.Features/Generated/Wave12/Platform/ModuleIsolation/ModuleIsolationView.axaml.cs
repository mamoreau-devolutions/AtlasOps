namespace AtlasOps.Features.Platform.ModuleIsolation;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ModuleIsolationView : UserControl
{
    public ModuleIsolationView()
    {
        this.DataContext = new ModuleIsolationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ModuleIsolationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}