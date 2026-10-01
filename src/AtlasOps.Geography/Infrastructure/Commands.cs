namespace AtlasOps.Geography.Infrastructure;

using System.Windows.Input;

public sealed class AsyncCommand : ICommand
{
    private readonly Func<Task> execute;
    private readonly Action<Exception> onError;
    private bool isExecuting;

    public AsyncCommand(Func<Task> execute, Action<Exception> onError)
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