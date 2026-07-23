using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CoverflowAltTab.Host.Debug;

public sealed class DebugWindowViewModel : INotifyPropertyChanged
{
    private const int MaxItems = 50;
    private string _appStatus = "Starting...";
    private string _hotkeyStatus = "Ctrl + Alt + Space";
    private string _sessionStatus = "Idle";
    private string _lastAction = "None";
    private string? _selectedRendererId;
    private string? _selectedAnimationId;

    public ObservableCollection<DebugKeyEventViewModel> RecentKeys { get; } = [];

    public ObservableCollection<DebugExtensionViewModel> Extensions { get; } = [];

    public ObservableCollection<string> AvailableRendererIds { get; } = [];

    public ObservableCollection<string> AvailableAnimationIds { get; } = [];

    public string AppStatus
    {
        get => _appStatus;
        set => SetField(ref _appStatus, value);
    }

    public string HotkeyStatus
    {
        get => _hotkeyStatus;
        set => SetField(ref _hotkeyStatus, value);
    }

    public string SessionStatus
    {
        get => _sessionStatus;
        set => SetField(ref _sessionStatus, value);
    }

    public string LastAction
    {
        get => _lastAction;
        set => SetField(ref _lastAction, value);
    }

    public string? SelectedRendererId
    {
        get => _selectedRendererId;
        set => SetField(ref _selectedRendererId, value);
    }

    public string? SelectedAnimationId
    {
        get => _selectedAnimationId;
        set => SetField(ref _selectedAnimationId, value);
    }

    public void AddKey(DebugKeyEventViewModel item)
    {
        RecentKeys.Insert(0, item);
        while (RecentKeys.Count > MaxItems)
        {
            RecentKeys.RemoveAt(RecentKeys.Count - 1);
        }
    }

    public void SetExtensions(IEnumerable<DebugExtensionViewModel> items)
    {
        Extensions.Clear();
        foreach (var item in items)
        {
            Extensions.Add(item);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
