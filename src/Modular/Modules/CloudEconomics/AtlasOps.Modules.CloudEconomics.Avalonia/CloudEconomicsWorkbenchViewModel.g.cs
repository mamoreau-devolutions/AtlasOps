namespace AtlasOps.Modules.CloudEconomics.Avalonia;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using AtlasOps.Modules.CloudEconomics.Core;

public sealed class CloudEconomicsWorkbenchViewModel : INotifyPropertyChanged
{
    private string _searchText = string.Empty; private CloudEconomicsCapabilityDescriptor? _selectedCapability;
    public string Title => CloudEconomicsModule.DisplayName;
    public string Summary => string.Concat(CloudEconomicsModule.Capabilities.Count, " operational capabilities across five business areas.");
    public IReadOnlyList<CloudEconomicsCapabilityDescriptor> Capabilities => string.IsNullOrWhiteSpace(_searchText) ? CloudEconomicsModule.Capabilities : CloudEconomicsModule.Capabilities.Where(item => item.DisplayName.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Area.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Concern.Contains(_searchText, StringComparison.OrdinalIgnoreCase)).ToArray();
    public string SearchText { get => _searchText; set { if (_searchText == value) { return; } _searchText = value ?? string.Empty; OnPropertyChanged(); OnPropertyChanged(nameof(Capabilities)); } }
    public CloudEconomicsCapabilityDescriptor? SelectedCapability { get => _selectedCapability; set { if (Equals(_selectedCapability, value)) { return; } _selectedCapability = value; OnPropertyChanged(); } }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
}