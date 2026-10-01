namespace AtlasOps.Features.Governance.DataClassification;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataClassificationView : UserControl
{
    public DataClassificationView()
    {
        this.DataContext = new DataClassificationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataClassificationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}