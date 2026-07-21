using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CoverflowAltTab.UI.Rendering;

namespace CoverflowAltTab.UI;

public sealed class OverlayViewModel : INotifyPropertyChanged
{
    private string _headerTitle = string.Empty;
    private string _headerSubtitle = string.Empty;
    private string _rendererId = string.Empty;

    public ObservableCollection<WindowListItemViewModel> Windows { get; } = [];

    public string HeaderTitle
    {
        get => _headerTitle;
        set
        {
            if (_headerTitle == value)
            {
                return;
            }

            _headerTitle = value;
            OnPropertyChanged();
        }
    }

    public string HeaderSubtitle
    {
        get => _headerSubtitle;
        set
        {
            if (_headerSubtitle == value)
            {
                return;
            }

            _headerSubtitle = value;
            OnPropertyChanged();
        }
    }

    public string RendererId
    {
        get => _rendererId;
        set
        {
            if (_rendererId == value)
            {
                return;
            }

            _rendererId = value;
            OnPropertyChanged();
        }
    }

    public void ShowModel(OverlayRenderModel model)
    {
        Windows.Clear();
        foreach (var item in model.Items)
        {
            Windows.Add(new WindowListItemViewModel
            {
                Title = item.Title,
                Subtitle = item.Subtitle,
                IsSelected = item.IsSelected,
            });
        }

        HeaderTitle = model.HeaderTitle;
        HeaderSubtitle = model.HeaderSubtitle;
        RendererId = model.RendererId;
    }

    public void UpdateSelection(int selectedIndex)
    {
        for (var index = 0; index < Windows.Count; index++)
        {
            Windows[index].IsSelected = index == selectedIndex;
        }
    }

    public void Clear()
    {
        Windows.Clear();
        HeaderTitle = string.Empty;
        HeaderSubtitle = string.Empty;
        RendererId = string.Empty;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
