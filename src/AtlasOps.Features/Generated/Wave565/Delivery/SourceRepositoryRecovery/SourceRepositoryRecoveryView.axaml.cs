namespace AtlasOps.Features.Delivery.SourceRepositoryRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SourceRepositoryRecoveryView : UserControl
{
    public SourceRepositoryRecoveryView()
    {
        this.DataContext = new SourceRepositoryRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SourceRepositoryRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}