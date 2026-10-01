namespace AtlasOps.Features.Platform.ModuleRegistration;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ModuleRegistrationView : UserControl
{
    public ModuleRegistrationView()
    {
        this.DataContext = new ModuleRegistrationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ModuleRegistrationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}