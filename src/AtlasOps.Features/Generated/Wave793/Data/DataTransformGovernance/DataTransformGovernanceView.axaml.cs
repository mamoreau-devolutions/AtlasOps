namespace AtlasOps.Features.Data.DataTransformGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataTransformGovernanceView : UserControl
{
    public DataTransformGovernanceView()
    {
        this.DataContext = new DataTransformGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataTransformGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}