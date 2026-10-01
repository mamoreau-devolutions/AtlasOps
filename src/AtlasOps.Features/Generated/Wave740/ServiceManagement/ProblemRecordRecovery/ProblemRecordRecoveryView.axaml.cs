namespace AtlasOps.Features.ServiceManagement.ProblemRecordRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ProblemRecordRecoveryView : UserControl
{
    public ProblemRecordRecoveryView()
    {
        this.DataContext = new ProblemRecordRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ProblemRecordRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}