namespace AtlasOps.Features.Sync.SchemaMigration;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SchemaMigrationView : UserControl
{
    public SchemaMigrationView()
    {
        this.DataContext = new SchemaMigrationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SchemaMigrationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}