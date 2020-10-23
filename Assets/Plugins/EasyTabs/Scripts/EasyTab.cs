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
    Text
}

public enum TextType
{
    BasicText,
    TextMeshPro
}

public enum StretchType
{
    Horizontal,
    Vertical,
    Full,
    CenterMiddle
}

public class EasyTab : MonoBehaviour
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
    [Tooltip("Which type this tab will be? A tab with images or a tab with texts?")]
    public TabType _tabType;
    [Tooltip("The texts for a text type tab, will be added from the left to right.")]
    public List<string> _strings;
    [Tooltip("The image assets for an image type tab, will be added from the left to right.")]
    public List<Sprite> _sprites;
    [Tooltip("Which type of text component will be used?")]
    public TextType _textType;


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

    [Tooltip("The color used on the selected tab buttons, for images or texts.")]
    public Color32 _selectedColor = new Color32(255, 255, 255, 255);
    [Tooltip("The color used on the unselected tab buttons, for images or texts.")]
    public Color32 _commonColor = new Color32(255, 255, 255, 255);

    public bool _dynamicGenerate;

    public GameObject _dynamicTab;

    public List<GameObject> _tabTexts;
    public List<GameObject> _tabImages;
    public List<GameObject> _tabs;

    private HorizontalLayoutGroup _horizonGroup = null;
    private VerticalLayoutGroup _verticalGroup = null;
    private Image _backImg = null;
    private int _testIndex;

    public void Init()
    {
        if(_tabLayout == TabLayout.Horizontal)
        {
            _horizonGroup = gameObject.GetComponent<HorizontalLayoutGroup>();
            if (!_horizonGroup) _horizonGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
            _horizonGroup.childControlHeight = false;
            _horizonGroup.childControlWidth = false;
            _horizonGroup.childAlignment = TextAnchor.MiddleCenter;
        }
        else
        {
            _verticalGroup = gameObject.GetComponent<VerticalLayoutGroup>();
            if (!_verticalGroup) _verticalGroup = gameObject.AddComponent<VerticalLayoutGroup>();
            _verticalGroup.childControlHeight = false;
            _verticalGroup.childControlWidth = false;
            _verticalGroup.childAlignment = TextAnchor.MiddleCenter;
        }

        
        //_horizonGroup.childForceExpandWidth = false;
        //_horizonGroup.childForceExpandHeight = false;
        if (_backgroud && !_backImg)
        {
            _backImg = gameObject.GetComponent<Image>();
            if(!_backImg) _backImg = gameObject.AddComponent<Image>();
            _backImg.sprite = _backgroud;
        }
        _tabs = new List<GameObject>();
        _tabTexts = new List<GameObject>();
        _tabImages = new List<GameObject>();
        _testIndex = 0;
    }

        
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

            if(_tabType == TabType.Image)
            {
                GameObject tabImage = new GameObject("TabImage");
                tabImage.transform.SetParent(tabButton.transform);
                Image img = tabImage.AddComponent<Image>();
                img.sprite = _sprites[i];
                _tabImages.Add(tabImage);
                tabImage.transform.localScale = Vector3.one;
            }
            else
            {
                GameObject tabText = new GameObject("TabText");
                tabText.transform.SetParent(tabButton.transform);
                if (_textType == TextType.BasicText)
                {
                    Text txt = tabText.AddComponent<Text>();
                    txt.text = _strings[i];
                    _tabTexts.Add(tabText);
                    tabText.transform.localScale = Vector3.one;
                }
                else
                {
                    TextMeshProUGUI txt = tabText.AddComponent<TextMeshProUGUI>();
                    txt.text = _strings[i];
                    txt.enableAutoSizing = true;
                    _tabTexts.Add(tabText);
                    tabText.transform.localScale = Vector3.one;
                }
            }
        }
    }

    /*
    public void GenerateTabsUGUI()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        Init();

        for (int i = 0; i < _tabCount; i++)
        {
            GameObject tab = new GameObject("Tab" + i);
            Button tabButton = tab.AddComponent<Button>();
            Image initImg = tab.AddComponent<Image>();
            initImg.enabled = false;

            RectTransform tabButtonRTranform = tabButton.GetComponent<RectTransform>();
            tab.transform.SetParent(transform);
            initImg.sprite = _tabOffSprite;
            tabButton.transition = Selectable.Transition.SpriteSwap;
            SpriteState btnSpriteState = new SpriteState();
            btnSpriteState = tabButton.spriteState;
            btnSpriteState.selectedSprite = _tabOnSprite;
            btnSpriteState.pressedSprite = _tabOnSprite;
            tabButton.spriteState = btnSpriteState;
            _tabs.Add(tab);
            tab.transform.localScale = Vector3.one;

            GameObject tabOn = new GameObject("TabOn");
            tabOn.transform.SetParent(tabButton.transform);
            Image onImg = tabOn.AddComponent<Image>();
            onImg.sprite = _tabOnSprite;
            onImg.type = Image.Type.Sliced;
            RectTransformStrech(tabOn, StretchType.Full);
            tabOn.transform.localScale = Vector3.one;

            GameObject tabOff = new GameObject("TabOff");
            tabOff.transform.SetParent(tabButton.transform);
            Image offImg = tabOff.AddComponent<Image>();
            offImg.sprite = _tabOffSprite;
            offImg.type = Image.Type.Sliced;
            RectTransformStrech(tabOff, StretchType.Full);
            tabOff.transform.localScale = Vector3.one;

            _tabWidth = tabButtonRTranform.sizeDelta.x;
            _tabHeight = tabButtonRTranform.sizeDelta.y;

            if (_tabType == TabType.Image)
            {
                GameObject tabImage = new GameObject("TabImage");
                tabImage.transform.SetParent(tabButton.transform);
                Image img = tabImage.AddComponent<Image>();
                img.sprite = _sprites[i];
                _tabImages.Add(tabImage);
                tabImage.transform.localScale = Vector3.one;
            }
            else
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
        //TabChildren = _tabs;
    }*/

    public void SetCustomItem()
    {
        if (_tabType == TabType.Image)
        {
            if (_tabImages.Count < 1)
                return;
            _customTab = _tabImages[0];
        }
        else
        {
            if (_tabTexts.Count < 1)
                return;
            _customTab = _tabTexts[0];
        }
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
        {
            if (_tabImages.Count >= _tabCount)
                return true;
            else
                return false;
        }
        else
        {
            if (_tabTexts.Count >= _tabCount)
                return true;
            else
                return false;
        }
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
        if (_tabType == TabType.Image)
        {
            for (int i = 0; i < _tabImages.Count; i ++)
            {
                _tabImages[i].GetComponent<Image>().sprite = _sprites[i];
            }
        }
        else
        {
            if(_textType == TextType.BasicText)
            {
                for (int i = 0; i < _tabTexts.Count; i++)
                {
                    _tabTexts[i].GetComponent<Text>().text = _strings[i];
                }
            }
            else
            {
                for (int i = 0; i < _tabTexts.Count; i++)
                {
                    _tabTexts[i].GetComponent<TextMeshProUGUI>().text = _strings[i];
                }
            }
        }
    }

    public void BrushOtherTabItems()
    {
#if UNITY_EDITOR
        if (_tabType == TabType.Image)
        {
            if (_tabImages.Count < 1)
                return;
                
            RectTransform customRt = _tabImages[0].GetComponent<RectTransform>();
            for (int i = 1; i < _tabImages.Count; i ++)
            {
                RectTransform tabImgRTranform = _tabImages[i].GetComponent<RectTransform>();
                UnityEditorInternal.ComponentUtility.CopyComponent(customRt);
                UnityEditorInternal.ComponentUtility.PasteComponentValues(tabImgRTranform);
            }
            _originalPos = customRt.anchoredPosition;
            //Debug.Log(_originalPos + "@@@@@@");
        }
        else
        {
            if (_tabTexts.Count < 1)
                return;

            RectTransform customRt = _tabTexts[0].GetComponent<RectTransform>();
            if (_textType == TextType.BasicText)
            {
                Text customTxt = _tabTexts[0].GetComponent<Text>();
                for (int i = 1; i < _tabTexts.Count; i++)
                {
                    RectTransform tabTxtRTranform = _tabTexts[i].GetComponent<RectTransform>();
                    UnityEditorInternal.ComponentUtility.CopyComponent(customRt);
                    UnityEditorInternal.ComponentUtility.PasteComponentValues(tabTxtRTranform);

                    Text tabTxt = _tabTexts[i].GetComponent<Text>();
                    string crtText = tabTxt.text;
                    UnityEditorInternal.ComponentUtility.CopyComponent(customTxt);
                    UnityEditorInternal.ComponentUtility.PasteComponentValues(tabTxt);
                    tabTxt.text = crtText;
                }
            }
            else
            {
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
            }
            _originalPos = customRt.anchoredPosition;
        }
#endif
    }

    public void SetFixedReady()
    {
        SelectedViewOf(0);
    }

    public void SimulateButtonView()
    {
        SelectedViewOf(_testIndex);
        _testIndex ++;
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
        _dynamicTab.SetActive(false);
    }

    //Dynamic Create button from design template
    public void CreateTabButton(Sprite icon, string text)
    {
        GameObject tabButton = Instantiate(_dynamicTab);
        tabButton.SetActive(true);
        tabButton.transform.SetParent(transform);
        _tabs.Add(tabButton);
        if (_tabType == TabType.Image)
        {
            Transform tabImgTransform = tabButton.transform.Find("TabImage");
            _tabImages.Add(tabImgTransform.gameObject);
            tabImgTransform.GetComponent<Image>().sprite = icon;
        }
        else
        {
            Transform tabTxtTransform = tabButton.transform.Find("TabText");
            _tabTexts.Add(tabTxtTransform.gameObject);
            if (_textType == TextType.BasicText)
            {
                tabTxtTransform.GetComponent<Text>().text = text;
            }
            else
            {
                tabTxtTransform.GetComponent<TextMeshProUGUI>().text = text;
            }
        }
    }

    //Register click event
    public void RegisterTabClickEvent(int index, Action onClick = null)
    {
        Button tabButton = _tabs[index].GetComponent<Button>();
        tabButton.onClick.AddListener(() => {
            onClick?.Invoke();
            SelectedViewOf(index);
        });  
    }

    public void SelectedViewOf(int index)
    {
        for (int i = 0; i < _tabs.Count; i ++)
        {
            if(i == index)
            {
                _tabs[i].transform.Find("TabOn").gameObject.SetActive(true);
                _tabs[i].transform.Find("TabOff").gameObject.SetActive(false);

                GameObject selectItem = null;
                if (_tabType == TabType.Image)
                {
                    Image img = _tabImages[i].GetComponent<Image>();
                    img.color = _selectedColor;
                    selectItem = _tabImages[i];
                }
                else
                {
                    if (_textType == TextType.BasicText)
                    {
                        Text tmp = _tabTexts[i].GetComponent<Text>();
                        tmp.color = _selectedColor;
                        selectItem = _tabTexts[i];
                    }
                    else
                    {
                        TextMeshProUGUI tmp = _tabTexts[i].GetComponent<TextMeshProUGUI>();
                        tmp.color = _selectedColor;
                        selectItem = _tabTexts[i];
                    }
                }
                if(_tabLayout == TabLayout.Horizontal)
                    selectItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(_originalPos.x, _originalPos.y + _jump);
                else
                    selectItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(_originalPos.x + _jump, _originalPos.y);
            }
            else
            {
                _tabs[i].transform.Find("TabOn").gameObject.SetActive(false);
                _tabs[i].transform.Find("TabOff").gameObject.SetActive(true);
                    
                GameObject unSelectItem = null;
                if (_tabType == TabType.Image)
                {
                    Image img = _tabImages[i].GetComponent<Image>();
                    img.color = _commonColor;
                    unSelectItem = _tabImages[i];
                }
                else
                {
                    if (_textType == TextType.BasicText)
                    {
                        Text tmp = _tabTexts[i].GetComponent<Text>();
                        tmp.color = _commonColor;
                        unSelectItem = _tabTexts[i];
                    }
                    else
                    {
                        TextMeshProUGUI tmp = _tabTexts[i].GetComponent<TextMeshProUGUI>();
                        tmp.color = _commonColor;
                        unSelectItem = _tabTexts[i];
                    }
                    
                }
                unSelectItem.GetComponent<RectTransform>().anchoredPosition = _originalPos;
                //Debug.Log(_originalPos + "22222@@@@@@");
            }
        }
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

public static class RectTransformExtensions
{
    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }

    public static float GetLeft(this RectTransform rt)
    {
        return rt.offsetMin.x;
    }

    public static float GetRight(this RectTransform rt)
    {
        return -rt.offsetMax.x;
    }

    public static float GetTop(this RectTransform rt)
    {
        return -rt.offsetMax.y;
    }

    public static float GetBottom(this RectTransform rt)
    {
        return rt.offsetMin.y;
    }
}