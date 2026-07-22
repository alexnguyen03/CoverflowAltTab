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
    private bool _usesFreeformLayout;
    private double _stageWidth;
    private double _stageHeight;
    private string _selectedBadgeTitle = string.Empty;
    private double _selectedPreviewX;
    private double _selectedPreviewY;
    private double _selectedPreviewWidth;
    private double _selectedPreviewHeight;
    private double _selectedPreviewOpacity = 1d;

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

    public bool UsesFreeformLayout
    {
        get => _usesFreeformLayout;
        set
        {
            if (_usesFreeformLayout == value)
            {
                return;
            }

            _usesFreeformLayout = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(UsesListLayout));
        }
    }

    public bool UsesListLayout => !UsesFreeformLayout;

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

    public string SelectedBadgeTitle
    {
        get => _selectedBadgeTitle;
        set
        {
            if (_selectedBadgeTitle == value)
            {
                return;
            }

            _selectedBadgeTitle = value;
            OnPropertyChanged();
        }
    }

    public double SelectedPreviewX
    {
        get => _selectedPreviewX;
        set
        {
            if (Math.Abs(_selectedPreviewX - value) < 0.01d)
            {
                return;
            }

            _selectedPreviewX = value;
            OnPropertyChanged();
        }
    }

    public double SelectedPreviewY
    {
        get => _selectedPreviewY;
        set
        {
            if (Math.Abs(_selectedPreviewY - value) < 0.01d)
            {
                return;
            }

            _selectedPreviewY = value;
            OnPropertyChanged();
        }
    }

    public double SelectedPreviewWidth
    {
        get => _selectedPreviewWidth;
        set
        {
            if (Math.Abs(_selectedPreviewWidth - value) < 0.01d)
            {
                return;
            }

            _selectedPreviewWidth = value;
            OnPropertyChanged();
        }
    }

    public double SelectedPreviewHeight
    {
        get => _selectedPreviewHeight;
        set
        {
            if (Math.Abs(_selectedPreviewHeight - value) < 0.01d)
            {
                return;
            }

            _selectedPreviewHeight = value;
            OnPropertyChanged();
        }
    }

    public double SelectedPreviewOpacity
    {
        get => _selectedPreviewOpacity;
        set
        {
            if (Math.Abs(_selectedPreviewOpacity - value) < 0.01d)
            {
                return;
            }

            _selectedPreviewOpacity = value;
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
        UsesFreeformLayout = model.UsesFreeformLayout;
        StageWidth = model.StageWidth;
        StageHeight = model.StageHeight;
        UpdateSelectedPreview(model);
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

        UsesFreeformLayout = model.UsesFreeformLayout;
        StageWidth = model.StageWidth;
        StageHeight = model.StageHeight;
        UpdateSelectedPreview(model);
    }

    public void Clear()
    {
        Windows.Clear();
        HeaderTitle = string.Empty;
        HeaderSubtitle = string.Empty;
        RendererId = string.Empty;
        UsesFreeformLayout = false;
        StageWidth = 0d;
        StageHeight = 0d;
        SelectedBadgeTitle = string.Empty;
        SelectedPreviewX = 0d;
        SelectedPreviewY = 0d;
        SelectedPreviewWidth = 0d;
        SelectedPreviewHeight = 0d;
        SelectedPreviewOpacity = 1d;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void UpdateSelectedPreview(OverlayRenderModel model)
    {
        if (model.SelectedIndex < 0 || model.SelectedIndex >= model.Items.Count)
        {
            SelectedBadgeTitle = string.Empty;
            SelectedPreviewX = 0d;
            SelectedPreviewY = 0d;
            SelectedPreviewWidth = 0d;
            SelectedPreviewHeight = 0d;
            SelectedPreviewOpacity = 1d;
            return;
        }

        var selectedItem = model.Items[model.SelectedIndex];
        SelectedBadgeTitle = selectedItem.Title;
        SelectedPreviewX = selectedItem.X;
        SelectedPreviewY = selectedItem.Y;
        SelectedPreviewWidth = selectedItem.Width;
        SelectedPreviewHeight = selectedItem.Height;
        SelectedPreviewOpacity = selectedItem.Opacity;
    }
}
