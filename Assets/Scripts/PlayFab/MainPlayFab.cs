using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.Json;
using PlayFab.ClientModels;

public class MainPlayFab : MonoBehaviour
{
    public static List<Level_NormalJob> level_NormalJob { get; set; }
    public static List<CatalogItem> Item_Catalog { get; set; }
    public static Dictionary<string, List<Item>> Item_Inventory_Dic { get; set; }
    public static List<Item> Item_Inventory0 { get; set; }
    public static List<Item> Item_Inventory1 { get; set; }
    public static List<Item> Item_Inventory2 { get; set; }
    public static GetTitleDataResult TitledataResult { get; set; }
    public static GetUserDataResult UserDataResult { get; set; }
    public static List<ItemInstance> Player_Inventry { get; set; }

    public static bool IsLogin { get; set; }
    public static bool IsAllDataGet { get; set; }

    public const int allDataCount=1;
    public static int GetDataCount { get; set; }

    public enum InventoryName
    {
        inventory0,
        inventory1,
        inventory2
    }
    private void Start()
    {
        level_NormalJob = new List<Level_NormalJob>();
        Item_Catalog = new List<CatalogItem>();
        Item_Inventory0 = new List<Item>();
        Item_Inventory1 = new List<Item>();
        Item_Inventory2 = new List<Item>();
        TitledataResult = new GetTitleDataResult();
        Item_Inventory_Dic = new Dictionary<string, List<Item>>();

        IsLogin = false;
        IsAllDataGet = false;

        GetDataCount = 0;

        LoginPlayFab();//ログイン処理
    }
    private void LoginPlayFab()
    {
        PlayFabClientAPI.LoginWithCustomID(
            new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true }
        , result => OnLogin()
        , error => Debug.Log("ログイン失敗"));
    }
    private void OnLogin()
    {
        IsLogin = true;
        print("Login");


        List<Action> methodList = new List<Action>();
        methodList.Add(() => GetItemData());
        methodList.Add(() => GetTitleData());
        methodList.Add(() => GetUserData());
        StartCoroutine(GetData(methodList));
    }
    public static IEnumerator GetData(List<Action> methodList)
    {

        GetDataCount = 0;
        int tempDataCount = 1;
        for(int i = 0; i < methodList.Count; i++)
        {
            methodList[i].Invoke();
            yield return new WaitUntil(() => tempDataCount == GetDataCount);
            tempDataCount++;
        }
        IsAllDataGet = true;

        //GetItemData();
        //yield return new WaitUntil(() => tempDataCount == GetDataCount);
        //tempDataCount++;
        //GetTitleData();
        //yield return new WaitUntil(() => tempDataCount == GetDataCount);
        //tempDataCount++;
        //GetUserData();
        //yield return new WaitUntil(() => tempDataCount == GetDataCount);
    }

    private void GetInventry()
    {
        List<ItemInstance> a;
        PlayFabClientAPI.GetUserInventory(
            new GetUserInventoryRequest()
            , result => a = result.Inventory
            , error => print(error.ErrorMessage));
    }

    /// <summary>
    /// アイテムの場所を移動させる処理
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    /// <param name="before"></param>
    /// <param name="after"></param>
    public static void ItemInventoryChange(int id,int count,string before,string after)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "incrementReadOnlyUserData",
            FunctionParameter = new { Id = id, Count = count, Before = before, After = after },
            GeneratePlayStreamEvent = true
        },
        result =>
        {
            print("成功");
            GetDataCount++;
        },
        error => print(error.ErrorMessage));
    }

    /// <summary>
    /// 倉庫内のアイテム取得
    /// </summary>
    public static void GetUserData()
    {
        PlayFabClientAPI.GetUserData(
            new GetUserDataRequest()
            , result =>
            {
                UserDataResult = result;
                var inventory = PlayFabSimpleJson.DeserializeObject<Inventory>(UserDataResult.Data["Inventory"].Value);
                Item_Inventory0 = inventory.inventory0;
                Item_Inventory1 = inventory.inventory1;
                Item_Inventory2 = inventory.inventory2;

                Item_Inventory_Dic[InventoryName.inventory0.ToString()] = Item_Inventory0;
                Item_Inventory_Dic[InventoryName.inventory1.ToString()] = Item_Inventory1;
                Item_Inventory_Dic[InventoryName.inventory2.ToString()] = Item_Inventory2;
                GetDataCount++;
            }
            , error => print(error.ErrorMessage));
    }

    /// <summary>
    /// ItemCatalog取得
    /// </summary>
    private void GetItemData()
    {
        PlayFabClientAPI.GetCatalogItems(
            new GetCatalogItemsRequest()
            , result =>
            {
                Item_Catalog = result.Catalog;
                GetDataCount++;
            }
            , error => Debug.Log(error.GenerateErrorReport()));
    }

    /// <summary>
    /// TitleDataをJson形式で取得
    /// </summary>
    private void GetTitleData( )
    {
        PlayFabClientAPI.GetTitleData(
            new GetTitleDataRequest()
            , result =>
            {
                TitledataResult = result;
                GetDataCount++;
            }
            , error => Debug.Log(error.GenerateErrorReport()));
    }

    /// <summary>
    /// タイトルデータをListに変換
    /// </summary>
    /// <typeparam name="T">Dataの型</typeparam>
    /// <param name="key">TitleDataのキー</param>
    /// <returns>キーのデータ</returns>
    private List<T> DeserializeTitleData<T>(string key)
    {
        return PlayFabSimpleJson.DeserializeObject<List<T>>(TitledataResult.Data[key]);
    }

    /// <summary>
    /// アイテム購入
    /// </summary>
    public static void BuyItem(string itemId)
    {
        CatalogItem item = Item_Catalog.Find(x => x.ItemId == itemId);

        PlayFabClientAPI.PurchaseItem(
            new PurchaseItemRequest { CatalogVersion = item.CatalogVersion, ItemId = item.ItemId, VirtualCurrency = "GD" ,Price=(int)item.VirtualCurrencyPrices["GD"]}
            , result => print("done")
            , error => print(error.ErrorMessage)) ;
    }
}
[Serializable]
public class Level_NormalJob
{
    public int Level;
    public int Exp_Difference;
    public int Exp_Cumulative;
    public int Sp_Difference;
    public int Sp_Cumulative;
}
[Serializable]
public class Inventory
{
    public List<Item> inventory0;
    public List<Item> inventory1;
    public List<Item> inventory2;
}
[Serializable]
public class Item
{
    public int Id;
    public int Count;
}