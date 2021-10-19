using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewController : MonoBehaviour
{
    public GameObject scrollView;
    public GameObject scrollContentPrefab;
    private void Start()
    {
        var viewPort = scrollView.transform.Find("Viewport");
        Transform content = viewPort.transform.Find("Content");

        // テキストを表示する
        for (int i = 0; i < MainPlayFab.Item_Catalog.Count; i++)
        {
            GameObject contents = Instantiate(scrollContentPrefab,content);
            contents.name = "Content" + (i+1);
            var butttonState = contents.GetComponent<Button>();
            // 背景の色を変えて見やすくする
            var colors = butttonState.colors;
            if (i % 2 == 0)
            {
                colors.normalColor = new Color(0F / 255F, 0F / 255F, 0F / 255F, 128F / 255F);
                colors.highlightedColor = new Color(0F / 255F, 0F / 255F, 0F / 255F, 128F / 255F);
                colors.pressedColor = new Color(0F / 255F, 0F / 255F, 0F / 255F, 128F / 255F);
                colors.disabledColor = new Color(0F / 255F, 0F / 255F, 0F / 255F, 128F / 255F);
            }
            else
            {
                colors.normalColor = new Color(0 / 255F, 0 / 255F, 0 / 255F, 50 / 255F);
                colors.highlightedColor = new Color(0 / 255F, 0 / 255F, 0 / 255F, 50 / 255F);
                colors.pressedColor = new Color(0 / 255F, 0 / 255F, 0 / 255F, 50 / 255F);
                colors.disabledColor = new Color(0 / 255F, 0 / 255F, 0 / 255F, 50 / 255F);
            }
            butttonState.colors = colors;


            var textChild = contents.GetComponentInChildren<Text>();
            textChild.text = MainPlayFab.Item_Catalog[i].DisplayName;
        }
    }
}
