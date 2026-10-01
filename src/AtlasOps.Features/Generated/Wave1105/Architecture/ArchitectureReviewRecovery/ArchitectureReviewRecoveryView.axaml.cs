namespace AtlasOps.Features.Architecture.ArchitectureReviewRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureReviewRecoveryView : UserControl
{
    public ArchitectureReviewRecoveryView()
    {
        this.DataContext = new ArchitectureReviewRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureReviewRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}