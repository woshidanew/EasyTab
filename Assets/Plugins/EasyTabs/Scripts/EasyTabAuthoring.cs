#if UNITY_EDITOR

using System.Collections.Generic;

using TMPro;

using UnityEngine;

using UnityEngine.UI;



/// <summary>

/// EasyTab 的编辑器授权部分：生成页签、Brush Format、各类 Ready 按钮等。

/// 这些逻辑只服务于编辑器流程，用 #if UNITY_EDITOR 包裹，不进运行时构建。

/// 与运行时部分共用同一个 EasyTab 类型（partial），编辑器仍按原方式调用这些方法。

/// </summary>

public partial class EasyTab

{

    private int _testIndex;



    /// <summary>

    /// 初始化布局组与背景，并重建页签节点列表。

    /// </summary>

    public void Init()

    {

        if (_tabLayout == TabLayout.Horizontal)

        {

            var horizonGroup = gameObject.GetComponent<HorizontalLayoutGroup>();

            if (!horizonGroup) horizonGroup = gameObject.AddComponent<HorizontalLayoutGroup>();

            horizonGroup.childControlHeight = false;

            horizonGroup.childControlWidth = false;

            horizonGroup.childAlignment = TextAnchor.MiddleCenter;

        }

        else

        {

            var verticalGroup = gameObject.GetComponent<VerticalLayoutGroup>();

            if (!verticalGroup) verticalGroup = gameObject.AddComponent<VerticalLayoutGroup>();

            verticalGroup.childControlHeight = false;

            verticalGroup.childControlWidth = false;

            verticalGroup.childAlignment = TextAnchor.MiddleCenter;

        }





        if (_backgroud)

        {

            var backImg = gameObject.GetComponent<Image>();

            if (!backImg) backImg = gameObject.AddComponent<Image>();

            backImg.sprite = _backgroud;

        }

        _tabs = new List<GameObject>();

        _tabTexts = new List<GameObject>();

        _tabImages = new List<GameObject>();

        _testIndex = 0;

    }



    /// <summary>

    /// 依 _tabCount 生成全部页签节点并写入运行时引用缓存。

    /// </summary>

    public void GenerateTabs()

    {

        while (transform.childCount > 0)

        {

            DestroyImmediate(transform.GetChild(0).gameObject);

        }



        Init();



        for (int i = 0; i < _tabCount; i++)

        {

            GameObject tabButton = new GameObject("Tab" + i);

            RectTransform tabButtonRTranform = tabButton.AddComponent<RectTransform>();



            //UGUI Generating

            tabButton.AddComponent<Button>();

            Image initImg = tabButton.AddComponent<Image>();

            initImg.enabled = false;



            tabButton.transform.SetParent(transform);

            tabButton.transform.localScale = Vector3.one;

            _tabs.Add(tabButton);



            GameObject tabOn = new GameObject("TabOn");

            tabOn.transform.SetParent(tabButton.transform);

            Image onImg = tabOn.AddComponent<Image>();

            onImg.sprite = _tabSelectedSprite;

            onImg.type = Image.Type.Sliced;

            RectTransformStrech(tabOn, StretchType.Full);

            tabOn.transform.localScale = Vector3.one;



            GameObject tabOff = new GameObject("TabOff");

            tabOff.transform.SetParent(tabButton.transform);

            Image offImg = tabOff.AddComponent<Image>();

            offImg.sprite = _tabCommonSprite;

            offImg.type = Image.Type.Sliced;

            RectTransformStrech(tabOff, StretchType.Full);

            tabOff.transform.localScale = Vector3.one;



            _tabWidth = tabButtonRTranform.sizeDelta.x;

            _tabHeight = tabButtonRTranform.sizeDelta.y;



            if (_tabType == TabType.Image || _tabType == TabType.ImageText)

            {

                GameObject tabImage = new GameObject("TabImage");

                tabImage.transform.SetParent(tabButton.transform);

                Image img = tabImage.AddComponent<Image>();

                img.sprite = _sprites[i];

                _tabImages.Add(tabImage);

                tabImage.transform.localScale = Vector3.one;

            }



            if (_tabType == TabType.Text || _tabType == TabType.ImageText)

            {

                GameObject tabText = new GameObject("TabText");

                tabText.transform.SetParent(tabButton.transform);

                TextMeshProUGUI txt = tabText.AddComponent<TextMeshProUGUI>();

                txt.text = _strings[i];

                txt.enableAutoSizing = true;

                _tabTexts.Add(tabText);

                tabText.transform.localScale = Vector3.one;

            }

        }



        RebuildTabItems();

    }



    public void SetCustomItem()

    {

        if (_tabImages.Count > 0)

            _customTab = _tabImages[0];

        else if (_tabTexts.Count > 0)

            _customTab = _tabTexts[0];

    }



    public bool HasTabFinishInit()

    {

        if (_tabs == null)

            return false;

        if (_tabs.Count >= _tabCount)

            return true;

        else

            return false;

    }



    public bool IsImagesORTextThere()

    {

        if (_tabType == TabType.Image)

            return _tabImages.Count >= _tabCount;

        if (_tabType == TabType.Text)

            return _tabTexts.Count >= _tabCount;

        return _tabImages.Count >= _tabCount && _tabTexts.Count >= _tabCount;

    }



    public void UpdateTabSize(float width, float height)

    {

        if (_tabs.Count < 1)

            return;

        foreach (GameObject tab in _tabs)

        {

            RectTransform tabButtonRTranform = tab.GetComponent<RectTransform>();

            tabButtonRTranform.sizeDelta = new Vector2(width, height);

        }

    }



    public void UpdateImageOrText()

    {

        if (_tabType == TabType.Image || _tabType == TabType.ImageText)

        {

            for (int i = 0; i < _tabImages.Count; i++)

            {

                _tabImages[i].GetComponent<Image>().sprite = _sprites[i];

            }

        }

        if (_tabType == TabType.Text || _tabType == TabType.ImageText)

        {

            for (int i = 0; i < _tabTexts.Count; i++)

            {

                _tabTexts[i].GetComponent<TextMeshProUGUI>().text = _strings[i];

            }

        }

    }



    public void BrushOtherTabItems()

    {

        if (_tabImages.Count > 0)

        {

            RectTransform customRt = _tabImages[0].GetComponent<RectTransform>();

            for (int i = 1; i < _tabImages.Count; i++)

            {

                RectTransform tabImgRTranform = _tabImages[i].GetComponent<RectTransform>();

                UnityEditorInternal.ComponentUtility.CopyComponent(customRt);

                UnityEditorInternal.ComponentUtility.PasteComponentValues(tabImgRTranform);

            }

            _originalPos = customRt.anchoredPosition;

        }



        if (_tabTexts.Count > 0)

        {

            RectTransform customRt = _tabTexts[0].GetComponent<RectTransform>();

            TextMeshProUGUI customTMP = _tabTexts[0].GetComponent<TextMeshProUGUI>();

            for (int i = 1; i < _tabTexts.Count; i++)

            {

                RectTransform tabTxtRTranform = _tabTexts[i].GetComponent<RectTransform>();

                UnityEditorInternal.ComponentUtility.CopyComponent(customRt);

                UnityEditorInternal.ComponentUtility.PasteComponentValues(tabTxtRTranform);



                TextMeshProUGUI tabTxtTMP = _tabTexts[i].GetComponent<TextMeshProUGUI>();

                string crtText = tabTxtTMP.text;

                UnityEditorInternal.ComponentUtility.CopyComponent(customTMP);

                UnityEditorInternal.ComponentUtility.PasteComponentValues(tabTxtTMP);

                tabTxtTMP.text = crtText;

            }

            if (_tabImages.Count > 0)

                _textOriginalPos = customRt.anchoredPosition;

            else

                _originalPos = customRt.anchoredPosition;

        }

    }



    public void SetFixedReady()

    {

        SelectedViewOf(0);

    }



    public void SimulateButtonView()

    {

        SelectedViewOf(_testIndex);

        _testIndex++;

        if (_testIndex >= _tabs.Count)

            _testIndex = 0;

    }



    public void SetDynamicReady()

    {

        _dynamicTab = _tabs[0];

        while (transform.childCount > 1)

        {

            DestroyImmediate(transform.GetChild(1).gameObject);

        }

        _tabs.Clear();

        _tabImages.Clear();

        _tabTexts.Clear();

        _tabItems.Clear();

        _dynamicTab.SetActive(false);

    }



    private void RectTransformStrech(GameObject go, StretchType type)

    {

        RectTransform rt = go.GetComponent<RectTransform>();

        rt.anchoredPosition = Vector2.zero;

        switch (type)

        {

            case StretchType.Horizontal:

                float crtHeight = rt.sizeDelta.y;

                rt.anchorMin = new Vector2(0, 0.5f);

                rt.anchorMax = new Vector2(1, 0.5f);

                rt.sizeDelta = new Vector2(0, crtHeight);

                break;

            case StretchType.Vertical:

                float crtWidth = rt.sizeDelta.x;

                rt.anchorMin = new Vector2(0.5f, 0);

                rt.anchorMax = new Vector2(0.5f, 1);

                rt.sizeDelta = new Vector2(crtWidth, 0);

                break;

            case StretchType.Full:

                rt.anchorMin = Vector2.zero;

                rt.anchorMax = Vector2.one;

                rt.sizeDelta = Vector2.zero;

                break;

            case StretchType.CenterMiddle:

                Vector2 midValue = new Vector2(0.5f, 0.5f);

                rt.anchorMin = midValue;

                rt.anchorMax = midValue;

                break;

        }

    }

}

#endif

