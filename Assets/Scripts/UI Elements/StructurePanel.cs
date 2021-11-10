using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StructurePanel : MonoBehaviour
{
    public static StructurePanel instance;
    [System.NonSerialized] public HorizontalLayoutGroup horizontalLayoutGroup;
    [System.NonSerialized] public int completedLayoutElement = 0;

    LevelAssetCreate levelAsset;

    private void Awake()
    {
        instance = this;
        horizontalLayoutGroup = transform.GetChild(0).GetComponent<HorizontalLayoutGroup>();
        levelAsset = Resources.Load<LevelAssetCreate>("Scriptables/LevelAsset");
    }

    public void CreateLayoutElement(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var createdLayoutElement = Instantiate(levelAsset.layoutElements, horizontalLayoutGroup.gameObject.transform);
            createdLayoutElement.name = "createdLayoutElement_" + i;
        }
        float paddingLeft = (levelAsset.layoutElements.GetComponent<RectTransform>().sizeDelta.x /2 ) * (count -1) * -1f + horizontalLayoutGroup.spacing/2 * (count - 1) * -1f;
        horizontalLayoutGroup.padding.left = (int) paddingLeft;
    }

    public void FillLayoutElement()
    {
        int childCount = horizontalLayoutGroup.transform.childCount;
        if(completedLayoutElement < childCount)
        {
            horizontalLayoutGroup.transform.GetChild(completedLayoutElement).GetChild(0).GetComponent<Image>().sprite = levelAsset.layoutElementComponents[1];
            completedLayoutElement++;
            if(completedLayoutElement == childCount)
            {
                LevelManager.instance.isLevelCompletedSuccesfully = true;
            }
        }
        
    }
}
