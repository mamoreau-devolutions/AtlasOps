namespace AtlasOps.Features.Hardening.RollbackPackage;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RollbackPackageView : UserControl
{
    public RollbackPackageView()
    {
        this.DataContext = new RollbackPackageViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RollbackPackageViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}