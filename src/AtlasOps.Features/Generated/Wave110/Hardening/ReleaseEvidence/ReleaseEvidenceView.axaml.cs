namespace AtlasOps.Features.Hardening.ReleaseEvidence;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseEvidenceView : UserControl
{
    public ReleaseEvidenceView()
    {
        this.DataContext = new ReleaseEvidenceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseEvidenceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}