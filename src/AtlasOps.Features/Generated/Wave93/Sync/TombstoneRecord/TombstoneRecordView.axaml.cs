namespace AtlasOps.Features.Sync.TombstoneRecord;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class TombstoneRecordView : UserControl
{
    public TombstoneRecordView()
    {
        this.DataContext = new TombstoneRecordViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is TombstoneRecordViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}