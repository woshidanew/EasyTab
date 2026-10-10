using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TabLayout
{
    Horizontal,
    Vertical
}

public enum TabType
{
    Image,
    Text,
    ImageText
}

public enum StretchType
{
    Horizontal,
    Vertical,
    Full,
    CenterMiddle
}

/// <summary>
/// 分页栏运行时组件。
/// 负责：保存配置、维护运行时引用缓存、在游戏运行时切换选中态表现。
/// 编辑器授权逻辑（生成页签、Brush Format、各类 Ready 按钮）拆分到 EasyTabAuthoring.cs，
/// 运行时缓存的数据结构见 TabItem.cs。
/// </summary>
public partial class EasyTab : MonoBehaviour
{
    [Tooltip("Which layout this tab will use? A horizontal one or a vertical one?")]
    public TabLayout _tabLayout;
    [Tooltip("Will this tab has a backgroud?")]
    public bool _hasBack = false;
    [Tooltip("Should fill it with the background asset. Can't be none.")]
    public Sprite _backgroud = null;
    [Tooltip("Should fill it with the tab selected asset. Can't be none.")]
    public Sprite _tabSelectedSprite = null;
    [Tooltip("Should fill it with the tab unselected asset. Can't be none.")]
    public Sprite _tabCommonSprite = null;
    [Tooltip("How many buttons will this tab have? Can't be less than 1.")]
    public int _tabCount = 1;
    [Tooltip("Which type this tab will be? A tab with images, texts, or both?")]
    public TabType _tabType;
    [Tooltip("The texts for a text or image-text type tab, will be added from the left to right.")]
    public List<string> _strings;
    [Tooltip("The image assets for an image or image-text type tab, will be added from the left to right.")]
    public List<Sprite> _sprites;


    [Tooltip("The width of one button in this tab.")]
    public float _tabWidth;
    [Tooltip("The height of one button in this tab.")]
    public float _tabHeight;

    [Tooltip("The GameObject needs customization work.")]
    public GameObject _customTab = null;

    [Tooltip("The distance of images or texts on tabs between selected or unselected states.")]
    public float _jump;
    [Tooltip("The position of the image or text on tab buttons in unselected state.")]
    public Vector2 _originalPos;
    [Tooltip("The position of the text on tab buttons in unselected state, only used by the image-text type.")]
    public Vector2 _textOriginalPos;

    [Tooltip("The color used on the selected tab buttons, for images or texts.")]
    public Color32 _selectedColor = new Color32(255, 255, 255, 255);
    [Tooltip("The color used on the unselected tab buttons, for images or texts.")]
    public Color32 _commonColor = new Color32(255, 255, 255, 255);
    [Tooltip("The color used on the selected tab text, only used by the image-text type.")]
    public Color32 _textSelectedColor = new Color32(255, 255, 255, 255);
    [Tooltip("The color used on the unselected tab text, only used by the image-text type.")]
    public Color32 _textCommonColor = new Color32(255, 255, 255, 255);

    public bool _dynamicGenerate;

    public GameObject _dynamicTab;

    [Tooltip("编辑器生成的页签文本节点列表（授权期使用，保留以兼容既有数据）。")]
    public List<GameObject> _tabTexts;
    [Tooltip("编辑器生成的页签图标节点列表（授权期使用，保留以兼容既有数据）。")]
    public List<GameObject> _tabImages;
    [Tooltip("编辑器生成的页签按钮节点列表（授权期使用，保留以兼容既有数据）。")]
    public List<GameObject> _tabs;

    /// <summary>
    /// 运行时引用缓存：由编辑器生成（或运行时兜底重建）后写入并序列化，
    /// 使 SelectedViewOf 等运行时代码不再使用 transform.Find / GetComponent。
    /// </summary>
    [SerializeField] private List<TabItem> _tabItems = new();

    private void Awake()
    {
        EnsureTabItems();
    }

    /// <summary>
    /// 依据 _tabs / _tabImages / _tabTexts 重建运行时引用缓存。
    /// 编辑器按钮、数据迁移（旧 prefab 尚无缓存）与运行时兜底都会调用。
    /// </summary>
    public void RebuildTabItems()
    {
        _tabItems.Clear();
        for (int i = 0; i < _tabs.Count; i++)
        {
            Transform tab = _tabs[i].transform;
            TabItem item = new()
            {
                button = _tabs[i].GetComponent<Button>(),
                tabOn = tab.Find("TabOn").gameObject,
                tabOff = tab.Find("TabOff").gameObject
            };
            if (i < _tabImages.Count)
            {
                item.image = _tabImages[i].GetComponent<Image>();
                item.imageRect = _tabImages[i].GetComponent<RectTransform>();
            }
            if (i < _tabTexts.Count)
            {
                item.text = _tabTexts[i].GetComponent<TextMeshProUGUI>();
                item.textRect = _tabTexts[i].GetComponent<RectTransform>();
            }
            _tabItems.Add(item);
        }
    }

    private void EnsureTabItems()
    {
        if (_tabItems.Count == _tabs.Count)
            return;
        RebuildTabItems();
    }

    /// <summary>
    /// 切换选中态：设置选中页签的表现（选中/未选中贴图显隐、颜色、jump 位移）。
    /// 运行时可安全高频调用，内部只访问已缓存的引用。
    /// </summary>
    public void SelectedViewOf(int index)
    {
        EnsureTabItems();
        CaptureTextBase();

        for (int i = 0; i < _tabs.Count; i++)
        {
            TabItem item = _tabItems[i];
            bool isSelected = i == index;
            item.tabOn.SetActive(isSelected);
            item.tabOff.SetActive(!isSelected);

            if (_tabType == TabType.Image || _tabType == TabType.ImageText)
            {
                item.image.color = isSelected ? _selectedColor : _commonColor;
                ApplyJump(item.imageRect, _originalPos, isSelected);
            }

            if (_tabType == TabType.Text || _tabType == TabType.ImageText)
            {
                bool imageText = _tabType == TabType.ImageText;
                item.text.color = imageText
                    ? (isSelected ? _textSelectedColor : _textCommonColor)
                    : (isSelected ? _selectedColor : _commonColor);
                ApplyJump(item.textRect, imageText ? _textOriginalPos : _originalPos, isSelected);
            }
        }
    }

    /// <summary>
    /// 为指定页签按钮注册点击事件：先执行外部回调，再切换到选中态。
    /// </summary>
    public void RegisterTabClickEvent(int index, Action onClick = null)
    {
        EnsureTabItems();
        _tabItems[index].button.onClick.AddListener(() =>
        {
            onClick?.Invoke();
            SelectedViewOf(index);
        });
    }

    /// <summary>
    /// 动态模式下，从 _dynamicTab 模板实例化一个页签按钮并写入缓存。
    /// </summary>
    public void CreateTabButton(Sprite icon, string text)
    {
        GameObject tabButton = Instantiate(_dynamicTab);
        tabButton.SetActive(true);
        tabButton.transform.SetParent(transform);
        _tabs.Add(tabButton);

        Transform tab = tabButton.transform;
        TabItem item = new()
        {
            button = tabButton.GetComponent<Button>(),
            tabOn = tab.Find("TabOn").gameObject,
            tabOff = tab.Find("TabOff").gameObject
        };
        if (_tabType == TabType.Image || _tabType == TabType.ImageText)
        {
            Transform tabImg = tab.Find("TabImage");
            _tabImages.Add(tabImg.gameObject);
            item.image = tabImg.GetComponent<Image>();
            item.imageRect = tabImg.GetComponent<RectTransform>();
            item.image.sprite = icon;
        }
        if (_tabType == TabType.Text || _tabType == TabType.ImageText)
        {
            Transform tabTxt = tab.Find("TabText");
            _tabTexts.Add(tabTxt.gameObject);
            item.text = tabTxt.GetComponent<TextMeshProUGUI>();
            item.textRect = tabTxt.GetComponent<RectTransform>();
            item.text.text = text;
        }
        _tabItems.Add(item);
    }

    /// <summary>
    /// 记录 ImageText 类型下文字的未选中基准坐标。
    /// 取当前处于未选中态的第一个 tab 作为来源，避免把已跳起的选中 tab 坐标误当成基准；
    /// 仅在基准仍为零（尚未配置）时写入，不会覆盖已经设置好的值。
    /// </summary>
    private void CaptureTextBase()
    {
        if (_tabType != TabType.ImageText || _textOriginalPos != Vector2.zero || _tabTexts.Count == 0)
            return;

        int source = -1;
        for (int i = 0; i < _tabs.Count; i++)
        {
            TabItem item = _tabItems[i];
            if (item.tabOn.activeSelf && !item.tabOff.activeSelf)
                continue;
            source = i;
            break;
        }
        if (source < 0)
            return;

        _textOriginalPos = _tabItems[source].textRect.anchoredPosition;
    }

    private void ApplyJump(RectTransform rt, Vector2 originalPos, bool isSelected)
    {
        if (!isSelected)
        {
            rt.anchoredPosition = originalPos;
            return;
        }
        rt.anchoredPosition = _tabLayout == TabLayout.Horizontal
            ? new Vector2(originalPos.x, originalPos.y + _jump)
            : new Vector2(originalPos.x + _jump, originalPos.y);
    }
}
