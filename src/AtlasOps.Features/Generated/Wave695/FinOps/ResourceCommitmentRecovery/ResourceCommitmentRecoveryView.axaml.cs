namespace AtlasOps.Features.FinOps.ResourceCommitmentRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ResourceCommitmentRecoveryView : UserControl
{
    public ResourceCommitmentRecoveryView()
    {
        this.DataContext = new ResourceCommitmentRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ResourceCommitmentRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}