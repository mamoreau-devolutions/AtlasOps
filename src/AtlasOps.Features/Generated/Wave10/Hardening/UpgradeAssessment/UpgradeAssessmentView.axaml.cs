namespace AtlasOps.Features.Hardening.UpgradeAssessment;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class UpgradeAssessmentView : UserControl
{
    public UpgradeAssessmentView()
    {
        this.DataContext = new UpgradeAssessmentViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is UpgradeAssessmentViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}