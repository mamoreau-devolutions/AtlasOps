namespace AtlasOps.Modules.Deployments.Avalonia;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using AtlasOps.Modules.Deployments.Core;

public sealed class DeploymentsWorkbenchViewModel : INotifyPropertyChanged
{
    private string _searchText = string.Empty; private DeploymentsCapabilityDescriptor? _selectedCapability;
    public string Title => DeploymentsModule.DisplayName;
    public string Summary => string.Concat(DeploymentsModule.Capabilities.Count, " operational capabilities across five business areas.");
    public IReadOnlyList<DeploymentsCapabilityDescriptor> Capabilities => string.IsNullOrWhiteSpace(_searchText) ? DeploymentsModule.Capabilities : DeploymentsModule.Capabilities.Where(item => item.DisplayName.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Area.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Concern.Contains(_searchText, StringComparison.OrdinalIgnoreCase)).ToArray();
    public string SearchText { get => _searchText; set { if (_searchText == value) { return; } _searchText = value ?? string.Empty; OnPropertyChanged(); OnPropertyChanged(nameof(Capabilities)); } }
    public DeploymentsCapabilityDescriptor? SelectedCapability { get => _selectedCapability; set { if (Equals(_selectedCapability, value)) { return; } _selectedCapability = value; OnPropertyChanged(); } }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
}