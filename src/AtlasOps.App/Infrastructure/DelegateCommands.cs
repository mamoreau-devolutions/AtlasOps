namespace AtlasOps.App.Infrastructure;

using System.Windows.Input;

public sealed class DelegateCommand : ICommand
{
    private readonly Action execute;
    private readonly Func<bool>? canExecute;

    public DelegateCommand(Action execute, Func<bool>? canExecute = null)
    {
        this.execute = execute;
        this.canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return this.canExecute?.Invoke() ?? true;
    }

    public void Execute(object? parameter)
    {
        this.execute();
    }

    public void NotifyCanExecuteChanged()
    {
        this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

public sealed class AsyncDelegateCommand : ICommand
{
    private readonly Func<Task> execute;
    private readonly Action<Exception> onError;
    private bool isExecuting;

    public AsyncDelegateCommand(Func<Task> execute, Action<Exception> onError)
    {
        this.execute = execute;
        this.onError = onError;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return !this.isExecuting;
    }

    public async void Execute(object? parameter)
    {
        if (this.isExecuting)
        {
            return;
        }

        this.isExecuting = true;
        this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        try
        {
            await this.execute();
        }
        catch (Exception exception)
        {
            this.onError(exception);
        }
        finally
        {
            this.isExecuting = false;
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}