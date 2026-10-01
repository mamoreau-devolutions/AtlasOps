namespace AtlasOps.Features.Automation.ConditionalStep;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ConditionalStepView : UserControl
{
    public ConditionalStepView()
    {
        this.DataContext = new ConditionalStepViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ConditionalStepViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}