using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.Json;
using PlayFab.ClientModels;
public class MainPlayFab : MonoBehaviour
{
    public static List<Level_NormalJob> Level_NormalJob { get; set; }
    public static List<CatalogItem> Item_Catalog { get; set; }
    private GetPlayerCombinedInfoRequestParams InfoRequestParams;
    private void Start()
    {
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
        GetItemData();
    }
    private void GetItemData()
    {
        PlayFabClientAPI.GetCatalogItems(
            new GetCatalogItemsRequest()
            , result =>
            {
                Item_Catalog = result.Catalog;
            }
            , error => Debug.Log(error.GenerateErrorReport()));
    }
    private void GetLevelData()
    {
        PlayFabClientAPI.GetTitleData(
            new GetTitleDataRequest()
            , result => 
            { 
                Level_NormalJob = PlayFabSimpleJson.DeserializeObject<List<Level_NormalJob>>(result.Data["Level_NormalJob"]);             
            }
            , error => Debug.Log(error.GenerateErrorReport()));
    }
}
[System.Serializable]
public class Level_NormalJob
{
    public int Level;
    public int Exp_Difference;
    public int Exp_Cumulative;
    public int Sp_Difference;
    public int Sp_Cumulative;
}