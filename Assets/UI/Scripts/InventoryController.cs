using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryController : MonoBehaviour
{
    /// <summary>
    /// アイテムの移動個数選択中
    /// </summary>
    public static bool isItemExplanation { get; set; }
    public static bool isDrag { get; set; }

    public GameObject scrollView_ItemCatalog;
    public GameObject ItemDisplay1;
    public GameObject ItemDisplay2;
    public GameObject ItemDisplayCover1;
    public GameObject ItemDisplayCover2;
    public GameObject scrollContentPrefab;
    private void Start()
    {
        SetItemCatalog();
        SetItem(ItemDisplay1, MainPlayFab.Item_Inventory0);
        SetItem(ItemDisplay2, MainPlayFab.Item_Inventory1);
    }
    private void SetItem(GameObject scrollView, List<Item> item_List)
    {
        Transform content = scrollView.transform.Find("Viewport").transform.Find("Content");
        if (0 < content.childCount)//子供が1以上あったら
        {
            var contentList = content.GetComponentsInChildren<Transform>().Skip(1);
            foreach (Transform v in contentList)//冗長だから嫌い対処法聞く
            {
                Destroy(v.gameObject);
            }
        }
        // テキストを表示する
        for (int i = 0; i < item_List.Count; i++)
        {
            var catalogItem = MainPlayFab.Item_Catalog.Find(x => int.Parse(x.ItemId) == item_List[i].Id);
            var item = item_List[i];

            GameObject contents = Instantiate(scrollContentPrefab, content);
            contents.name = "Content" + (i + 1);

            var button = contents.GetComponent<Button>();
            var itemDrag = contents.GetComponent<ItemDrag>();
            var itemName_Text = contents.transform.Find("ItemName").GetComponent<Text>();
            var itemCount_Text = contents.transform.Find("ItemCount").GetComponent<Text>();

            int ii = i;
            //button.onClick.AddListener(() => ItemOnClick(item_List[ii].Id));//アイテムをクリックされたときの処理

            itemDrag.nowFrame_Obj = scrollView;
            itemDrag.item = item;
            itemDrag.inventoryController = this;

            itemName_Text.text = catalogItem.DisplayName;
            itemCount_Text.text = "×" + item.Count.ToString();
        }
    }
    private void SetItemCatalog()
    {
        Transform content = scrollView_ItemCatalog.transform.Find("Viewport").transform.Find("Content");

        // テキストを表示する
        for (int i = 0; i < MainPlayFab.Item_Catalog.Count; i++)
        {
            GameObject contents = Instantiate(scrollContentPrefab, content);
            contents.name = "Content" + (i + 1);

            var button = contents.GetComponent<Button>();
            int ii = i + 1;
            button.onClick.AddListener(() => ItemOnClick(ii));

            var textChild = contents.GetComponentInChildren<Text>();
            textChild.text = MainPlayFab.Item_Catalog[i].DisplayName;
        }
    }
    public void InventoryChange(Dropdown dropdown)
    {
        var frame_dropDown = Singleton.Frame_DropDown.FirstOrDefault(e => e.Value == dropdown.gameObject);
        var item_listName = (MainPlayFab.InventoryName)dropdown.value;
        var item_list = MainPlayFab.Item_Inventory_Dic[item_listName.ToString()];
        SetItem(frame_dropDown.Key, item_list);
    }
    private void ItemOnClick(int itemId)
    {
        var item = MainPlayFab.Item_Catalog.Find(x => int.Parse(x.ItemId) == itemId);
        print(item.DisplayName);
    }
    public void ItemOnEndDrag(ItemDrag callerObj, PointerEventData eventData, GameObject nowFrame_Obj, Item item, Vector3 prePosition)
    {
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        if (raycastResults.Any(element => element.gameObject != nowFrame_Obj && element.gameObject.name.Contains("Inventory")))//ドロップが今と違う場所なら
        {
            var hit = raycastResults.Find(element => element.gameObject != nowFrame_Obj && element.gameObject.name.Contains("Inventory"));//ドロップ先を取得
            callerObj.transform.position = hit.gameObject.transform.position;//座標を移動

            //アイテム数の処理
            Dropdown nowDropDown = Singleton.Frame_DropDown[nowFrame_Obj].GetComponent<Dropdown>();//今格納されている場所
            Dropdown nextDropDown = Singleton.Frame_DropDown[hit.gameObject].GetComponent<Dropdown>();//次格納される場所

            string nowInv = GetInventoryName(nowFrame_Obj);
            string nextInv = GetInventoryName(hit.gameObject);

            print(nowInv);
            print(nextInv);

            List<Action> methodList = new List<Action>
            {
                () => MainPlayFab.ItemInventoryChange(item.Id, item.Count, nowInv, nextInv),//アイテムの場所変更
                () => MainPlayFab.GetUserData()
            };

            StartCoroutine(MainPlayFab.GetData(methodList));

            callerObj.nowFrame_Obj = hit.gameObject;//現在の枠更新
        }
        else
        {
            callerObj.transform.position = prePosition;
        }
        callerObj.transform.SetParent(callerObj.nowFrame_Obj.transform.Find("Viewport/Content"), true);//親子関係設定
        isDrag = false;
    }
    private static string GetInventoryName(GameObject frame)
    {
        Dropdown dropDown = Singleton.Frame_DropDown[frame].GetComponent<Dropdown>();
        MainPlayFab.InventoryName name = (MainPlayFab.InventoryName)dropDown.value;
        return name.ToString();
    }
    public  void ItemClick()
    {
        if (!isDrag)
        {
            ItemDisplayCover1.SetActive(false);
            ItemDisplayCover2.SetActive(false);

        }
    }
}