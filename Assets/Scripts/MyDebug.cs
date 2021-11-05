using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
public class MyDebug : MonoBehaviour
{
    public GameObject canvas;
    public GameObject displayFrame1;
    public GameObject displayFrame2;
    public GameObject displayDropDown1;
    public GameObject displayDropDown2;
    private void Awake()
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            SceneManager.LoadScene("Title");
        }
    }
    private void Start()
    {
        Singleton.MainCanvas = canvas;
        Singleton.Frame_DropDown = new Dictionary<GameObject, GameObject>();
        Singleton.Frame_DropDown.Add(displayFrame1, displayDropDown1);
        Singleton.Frame_DropDown.Add(displayFrame2, displayDropDown2);
    }
}
