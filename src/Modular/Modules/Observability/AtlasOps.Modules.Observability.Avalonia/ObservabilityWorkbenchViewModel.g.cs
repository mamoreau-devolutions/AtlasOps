namespace AtlasOps.Modules.Observability.Avalonia;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using AtlasOps.Modules.Observability.Core;

public sealed class ObservabilityWorkbenchViewModel : INotifyPropertyChanged
{
    private string _searchText = string.Empty; private ObservabilityCapabilityDescriptor? _selectedCapability;
    public string Title => ObservabilityModule.DisplayName;
    public string Summary => string.Concat(ObservabilityModule.Capabilities.Count, " operational capabilities across five business areas.");
    public IReadOnlyList<ObservabilityCapabilityDescriptor> Capabilities => string.IsNullOrWhiteSpace(_searchText) ? ObservabilityModule.Capabilities : ObservabilityModule.Capabilities.Where(item => item.DisplayName.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Area.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Concern.Contains(_searchText, StringComparison.OrdinalIgnoreCase)).ToArray();
    public string SearchText { get => _searchText; set { if (_searchText == value) { return; } _searchText = value ?? string.Empty; OnPropertyChanged(); OnPropertyChanged(nameof(Capabilities)); } }
    public ObservabilityCapabilityDescriptor? SelectedCapability { get => _selectedCapability; set { if (Equals(_selectedCapability, value)) { return; } _selectedCapability = value; OnPropertyChanged(); } }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
}