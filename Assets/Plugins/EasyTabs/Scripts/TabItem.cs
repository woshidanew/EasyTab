using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 单个页签的运行时引用缓存。
/// 由 EasyTab 在编辑器生成期写入并随 prefab/场景序列化，
/// 或由运行时兜底重建，使 SelectedViewOf 等运行时路径不再 transform.Find / GetComponent。
/// 与 EasyTab 的 _tabs / _tabImages / _tabTexts 按索引一一对应。
/// </summary>
[Serializable]
public class TabItem
{
    /// <summary>页签按钮组件。</summary>
    public Button button;

    /// <summary>选中态节点（TabOn）。</summary>
    public GameObject tabOn;

    /// <summary>未选中态节点（TabOff）。</summary>
    public GameObject tabOff;

    /// <summary>图标节点组件（Image / ImageText 类型才有）。</summary>
    public Image image;

    /// <summary>图标节点的 RectTransform（Image / ImageText 类型才有）。</summary>
    public RectTransform imageRect;

    /// <summary>文字组件（Text / ImageText 类型才有）。</summary>
    public TextMeshProUGUI text;

    /// <summary>文字节点的 RectTransform（Text / ImageText 类型才有）。</summary>
    public RectTransform textRect;
}
