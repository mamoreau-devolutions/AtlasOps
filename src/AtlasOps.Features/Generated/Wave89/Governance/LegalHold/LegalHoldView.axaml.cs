namespace AtlasOps.Features.Governance.LegalHold;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class LegalHoldView : UserControl
{
    public LegalHoldView()
    {
        this.DataContext = new LegalHoldViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is LegalHoldViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}