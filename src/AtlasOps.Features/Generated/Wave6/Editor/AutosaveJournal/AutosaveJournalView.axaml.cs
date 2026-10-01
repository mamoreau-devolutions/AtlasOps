namespace AtlasOps.Features.Editor.AutosaveJournal;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AutosaveJournalView : UserControl
{
    public AutosaveJournalView()
    {
        this.DataContext = new AutosaveJournalViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AutosaveJournalViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}