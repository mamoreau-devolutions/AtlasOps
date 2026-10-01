namespace AtlasOps.Core;

using System.ComponentModel;
using System.Runtime.CompilerServices;

public sealed record AtlasOpsFieldDescriptor(string Name, string TypeName, bool IsEditable);

public sealed record AtlasOpsModelDescriptor(
    string TypeName,
    string Category,
    string DisplayName,
    IReadOnlyList<AtlasOpsFieldDescriptor> Fields);

public interface IGeneratedEditorViewModel : INotifyPropertyChanged
{
    object Model { get; }

    string ModelType { get; }

    string DisplayName { get; }

    string EditorCategory { get; }

    bool HasChanges { get; }

    IReadOnlyList<GeneratedFieldEditorViewModel> Fields { get; }

    void AcceptChanges();
}

public sealed class GeneratedFieldEditorViewModel : INotifyPropertyChanged
{
    private readonly Func<string> read;
    private readonly Func<string, bool> write;
    private string validationMessage = string.Empty;

    public GeneratedFieldEditorViewModel(
        string name,
        string typeName,
        Func<string> read,
        Func<string, bool> write)
    {
        this.Name = name;
        this.TypeName = typeName;
        this.read = read;
        this.write = write;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name { get; }

    public string TypeName { get; }

    public string Value
    {
        get => this.read();
        set
        {
            if (this.write(value))
            {
                this.ValidationMessage = string.Empty;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Value)));
                return;
            }

            this.ValidationMessage = $"Enter a valid {this.TypeName}.";
        }
    }

    public string ValidationMessage
    {
        get => this.validationMessage;
        private set
        {
            if (this.validationMessage == value)
            {
                return;
            }

            this.validationMessage = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ValidationMessage)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.HasError)));
        }
    }

    public bool HasError => !string.IsNullOrEmpty(this.ValidationMessage);
}

public abstract class GeneratedEditorViewModelBase<TModel> : IGeneratedEditorViewModel
    where TModel : class, IAtlasOpsEntity
{
    private bool isDirty;

    protected GeneratedEditorViewModelBase(TModel model, string displayName, string category)
    {
        this.TypedModel = model;
        this.DisplayName = displayName;
        this.EditorCategory = category;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public TModel TypedModel { get; }

    public object Model => this.TypedModel;

    public string ModelType => typeof(TModel).Name;

    public string DisplayName { get; }

    public string EditorCategory { get; }

    public bool HasChanges
    {
        get => this.isDirty;
        private set
        {
            if (this.isDirty == value)
            {
                return;
            }

            this.isDirty = value;
            this.OnPropertyChanged();
        }
    }

    public abstract IReadOnlyList<GeneratedFieldEditorViewModel> Fields { get; }

    public void AcceptChanges()
    {
        this.HasChanges = false;
    }

    protected void SetProperty<TValue>(
        TValue currentValue,
        TValue newValue,
        Action<TValue> apply,
        [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<TValue>.Default.Equals(currentValue, newValue))
        {
            return;
        }

        apply(newValue);
        this.TypedModel.UpdatedAt = DateTimeOffset.UtcNow;
        this.HasChanges = true;
        this.OnPropertyChanged(propertyName);
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetString(string value, Action<string> apply, [CallerMemberName] string propertyName = "")
    {
        apply(value);
        this.MarkChanged(propertyName);
        return true;
    }

    protected bool SetBoolean(string value, Action<bool> apply, [CallerMemberName] string propertyName = "")
    {
        if (!bool.TryParse(value, out bool parsedValue))
        {
            return false;
        }

        apply(parsedValue);
        this.MarkChanged(propertyName);
        return true;
    }

    protected bool SetInteger(string value, Action<int> apply, [CallerMemberName] string propertyName = "")
    {
        if (!int.TryParse(value, out int parsedValue))
        {
            return false;
        }

        apply(parsedValue);
        this.MarkChanged(propertyName);
        return true;
    }

    protected bool SetLong(string value, Action<long> apply, [CallerMemberName] string propertyName = "")
    {
        if (!long.TryParse(value, out long parsedValue))
        {
            return false;
        }

        apply(parsedValue);
        this.MarkChanged(propertyName);
        return true;
    }

    protected bool SetDouble(string value, Action<double> apply, [CallerMemberName] string propertyName = "")
    {
        if (!double.TryParse(value, out double parsedValue))
        {
            return false;
        }

        apply(parsedValue);
        this.MarkChanged(propertyName);
        return true;
    }

    protected bool SetDecimal(string value, Action<decimal> apply, [CallerMemberName] string propertyName = "")
    {
        if (!decimal.TryParse(value, out decimal parsedValue))
        {
            return false;
        }

        apply(parsedValue);
        this.MarkChanged(propertyName);
        return true;
    }

    protected bool SetDateTimeOffset(string value, Action<DateTimeOffset> apply, [CallerMemberName] string propertyName = "")
    {
        if (!DateTimeOffset.TryParse(value, out DateTimeOffset parsedValue))
        {
            return false;
        }

        apply(parsedValue);
        this.MarkChanged(propertyName);
        return true;
    }

    private void MarkChanged(string propertyName)
    {
        this.TypedModel.UpdatedAt = DateTimeOffset.UtcNow;
        this.HasChanges = true;
        this.OnPropertyChanged(propertyName);
    }
}

public sealed record AtlasOpsEntitySummary(
    IAtlasOpsEntity Entity,
    string TypeName,
    string Category,
    string DisplayName,
    string PrimaryText,
    string SecondaryText,
    string SearchText);

public sealed class AtlasOpsGeneratedWorkspace
{
    public AtlasOpsGeneratedWorkspace(IReadOnlyList<IAtlasOpsEntity> entities)
    {
        this.Entities = entities;
    }

    public IReadOnlyList<IAtlasOpsEntity> Entities { get; }
}