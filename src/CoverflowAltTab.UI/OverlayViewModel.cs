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
    private OverlayLayoutKind _layoutKind = OverlayLayoutKind.List;
    private double _stageWidth;
    private double _stageHeight;
    private string _selectedTitle = string.Empty;

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

    public OverlayLayoutKind LayoutKind
    {
        get => _layoutKind;
        set
        {
            if (_layoutKind == value)
            {
                return;
            }

            _layoutKind = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(UsesFreeformLayout));
            OnPropertyChanged(nameof(UsesListLayout));
        }
    }

    public bool UsesFreeformLayout => LayoutKind != OverlayLayoutKind.List;

    public bool UsesListLayout => LayoutKind == OverlayLayoutKind.List;

    public double StageWidth
    {
        get => _stageWidth;
        set
        {
            if (Math.Abs(_stageWidth - value) < 0.01d)
            {
                return;
            }

            _stageWidth = value;
            OnPropertyChanged();
        }
    }

    public double StageHeight
    {
        get => _stageHeight;
        set
        {
            if (Math.Abs(_stageHeight - value) < 0.01d)
            {
                return;
            }

            _stageHeight = value;
            OnPropertyChanged();
        }
    }

    public string SelectedTitle
    {
        get => _selectedTitle;
        set
        {
            if (_selectedTitle == value)
            {
                return;
            }

            _selectedTitle = value;
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
                WindowHandle = item.WindowHandle,
                IsSelected = item.IsSelected,
                X = item.X,
                Y = item.Y,
                Width = item.Width,
                Height = item.Height,
                Scale = item.Scale,
                Rotation = item.Rotation,
                Opacity = item.Opacity,
                ZIndex = item.ZIndex,
            });
        }

        HeaderTitle = model.HeaderTitle;
        HeaderSubtitle = model.HeaderSubtitle;
        RendererId = model.RendererId;
        LayoutKind = model.LayoutKind;
        StageWidth = model.StageWidth;
        StageHeight = model.StageHeight;
        SelectedTitle = GetSelectedTitle(model);
    }

    public void UpdateSelection(OverlayRenderModel model)
    {
        for (var index = 0; index < Windows.Count; index++)
        {
            var source = model.Items[index];
            var target = Windows[index];
            target.IsSelected = source.IsSelected;
            target.X = source.X;
            target.Y = source.Y;
            target.Width = source.Width;
            target.Height = source.Height;
            target.Scale = source.Scale;
            target.Rotation = source.Rotation;
            target.Opacity = source.Opacity;
            target.ZIndex = source.ZIndex;
        }

        LayoutKind = model.LayoutKind;
        StageWidth = model.StageWidth;
        StageHeight = model.StageHeight;
        SelectedTitle = GetSelectedTitle(model);
    }

    public void Clear()
    {
        Windows.Clear();
        HeaderTitle = string.Empty;
        HeaderSubtitle = string.Empty;
        RendererId = string.Empty;
        LayoutKind = OverlayLayoutKind.List;
        StageWidth = 0d;
        StageHeight = 0d;
        SelectedTitle = string.Empty;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private static string GetSelectedTitle(OverlayRenderModel model)
    {
        return model.SelectedIndex >= 0 && model.SelectedIndex < model.Items.Count
            ? model.Items[model.SelectedIndex].Title
            : string.Empty;
    }
}
