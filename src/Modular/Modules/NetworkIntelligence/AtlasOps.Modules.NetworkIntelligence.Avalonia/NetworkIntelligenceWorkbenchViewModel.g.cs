namespace AtlasOps.Modules.NetworkIntelligence.Avalonia;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using AtlasOps.Modules.NetworkIntelligence.Core;

public sealed class NetworkIntelligenceWorkbenchViewModel : INotifyPropertyChanged
{
    private string _searchText = string.Empty; private NetworkIntelligenceCapabilityDescriptor? _selectedCapability;
    public string Title => NetworkIntelligenceModule.DisplayName;
    public string Summary => string.Concat(NetworkIntelligenceModule.Capabilities.Count, " operational capabilities across five business areas.");
    public IReadOnlyList<NetworkIntelligenceCapabilityDescriptor> Capabilities => string.IsNullOrWhiteSpace(_searchText) ? NetworkIntelligenceModule.Capabilities : NetworkIntelligenceModule.Capabilities.Where(item => item.DisplayName.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Area.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Concern.Contains(_searchText, StringComparison.OrdinalIgnoreCase)).ToArray();
    public string SearchText { get => _searchText; set { if (_searchText == value) { return; } _searchText = value ?? string.Empty; OnPropertyChanged(); OnPropertyChanged(nameof(Capabilities)); } }
    public NetworkIntelligenceCapabilityDescriptor? SelectedCapability { get => _selectedCapability; set { if (Equals(_selectedCapability, value)) { return; } _selectedCapability = value; OnPropertyChanged(); } }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
}