namespace AtlasOps.Features.Hardening.DiagnosticSnapshot;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DiagnosticSnapshotView : UserControl
{
    public DiagnosticSnapshotView()
    {
        this.DataContext = new DiagnosticSnapshotViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DiagnosticSnapshotViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}