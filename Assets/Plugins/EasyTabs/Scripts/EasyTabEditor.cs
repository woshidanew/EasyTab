using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(EasyTab))]
public class EasyTabEditor : UnityEditor.Editor
{
    public SerializedObject mObject;

    public SerializedProperty mTabLayout;

    public SerializedProperty mHasBack;
    public SerializedProperty mBackgroud;

    public SerializedProperty mTabOnSprite;
    public SerializedProperty mTabOffSprite;
    public SerializedProperty mTabCount;
    public SerializedProperty mTabType;
    public SerializedProperty mStrings;
    public SerializedProperty mSprites;
    public SerializedProperty mTextType;

    public SerializedProperty mTabWith;
    public SerializedProperty mTabHeight;

    public SerializedProperty mCustomGo;
    public SerializedProperty mJump;
    public SerializedProperty mOriginalPos;

    public SerializedProperty mOnColor;
    public SerializedProperty mOffColor;

    public SerializedProperty mIsDynamic;
    public SerializedProperty mDynamicTab;

    public void OnEnable()
    {
        mObject = new SerializedObject(target);

        mTabLayout = mObject.FindProperty("_tabLayout");
        mHasBack = mObject.FindProperty("_hasBack");
        mBackgroud = mObject.FindProperty("_backgroud");

        mTabOnSprite = mObject.FindProperty("_tabSelectedSprite");
        mTabOffSprite = mObject.FindProperty("_tabCommonSprite");
        mTabCount = mObject.FindProperty("_tabCount");
        mCustomGo = mObject.FindProperty("_customTab");

        mTabWith = mObject.FindProperty("_tabWidth");
        mTabHeight = mObject.FindProperty("_tabHeight");
        mJump = mObject.FindProperty("_jump");

        mTabType = mObject.FindProperty("_tabType");
        mStrings = mObject.FindProperty("_strings");
        mSprites = mObject.FindProperty("_sprites");
        mTextType = mObject.FindProperty("_textType");

        mOnColor = mObject.FindProperty("_selectedColor");
        mOffColor = mObject.FindProperty("_commonColor");
        mOriginalPos = mObject.FindProperty("_originalPos");

        mIsDynamic = mObject.FindProperty("_dynamicGenerate");
        mDynamicTab = mObject.FindProperty("_dynamicTab");
    }

    public override void OnInspectorGUI()
    {
        mObject.Update();
        EasyTab mScript = (EasyTab)target;

        EditorGUILayout.PropertyField(mTabLayout);
        EditorGUILayout.PropertyField(mHasBack);
        if (mHasBack.boolValue)
            EditorGUILayout.PropertyField(mBackgroud);

        EditorGUILayout.PropertyField(mTabOnSprite);
        EditorGUILayout.PropertyField(mTabOffSprite);
        EditorGUILayout.PropertyField(mTabCount);

        EditorGUILayout.PropertyField(mTabType);
        switch (mTabType.enumValueIndex)
        {
            case 0:
                mSprites.arraySize = mTabCount.intValue;
                mSprites.isExpanded = true;
                EditorGUILayout.PropertyField(mSprites);
                break;
            case 1:
                mStrings.arraySize = mTabCount.intValue;
                mStrings.isExpanded = true;
                EditorGUILayout.PropertyField(mStrings);
                EditorGUILayout.PropertyField(mTextType);
                break;
        }
        if (mScript.HasTabFinishInit() && mScript.IsImagesORTextThere())
        {
            mScript.UpdateImageOrText();
        }

        if (GUILayout.Button("Spread Out"))
        {
            if(mTabOnSprite.objectReferenceValue == null || mTabOffSprite.objectReferenceValue == null)
            {
                Debug.Log("Oops! Looks like you haven't added the images for tabs.");
                return;
            }
            if (mTabCount.intValue <= 0)
            {
                Debug.Log("Oops! Looks like you don't want to make even 1 tab.");
                return;
            }
            mScript.GenerateTabs();
            mScript.SetCustomItem();
            mObject.Update();
            Debug.Log("Good! Tab assets finish initialization, start to customize it!");
        }

        EditorGUILayout.Slider(mTabWith, 0, 1080);
        EditorGUILayout.Slider(mTabHeight, 0, 1080);
        EditorGUILayout.PropertyField(mCustomGo);

        if (GUI.changed && mScript.HasTabFinishInit())
        {
            mScript.UpdateTabSize(mTabWith.floatValue, mTabHeight.floatValue);
        }

        if (GUILayout.Button("Brush Format"))
        {
            mScript.BrushOtherTabItems();
            Debug.Log("Great! All tabs copied your customization!");
        }

        EditorGUILayout.PropertyField(mOriginalPos);
        EditorGUILayout.PropertyField(mJump);
        EditorGUILayout.PropertyField(mOnColor);
        EditorGUILayout.PropertyField(mOffColor);

        if (GUILayout.Button("Set Fixed Ready"))
        {
            mScript.SetFixedReady();
            Debug.Log("Good job! It seems that this static tab is done! Make it a prefab or just leave it here and let the programmers know it!");
        }

        //if(mTabBase.enumValueIndex == 0)
        //{
            if (GUILayout.Button("Simulation Test"))
            {
                mScript.SimulateButtonView();
            }
        //}
        EditorGUILayout.PropertyField(mIsDynamic);
        mObject.ApplyModifiedProperties();
        if (mIsDynamic.boolValue)
        {
            EditorGUILayout.PropertyField(mDynamicTab);
            if (GUILayout.Button("Set Dynamic Ready"))
            {
                mScript.SetDynamicReady();
            }
        }
    }
}

