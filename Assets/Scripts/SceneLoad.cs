using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
    private bool isLoadClick;
    public void Load(string scene)
    {
        if (isLoadClick) return;
        var async=SceneManager.LoadSceneAsync(scene);
        async.allowSceneActivation = false;
        StartCoroutine(IsLogin());
        IEnumerator IsLogin()
        {
            yield return new WaitUntil(() => MainPlayFab.IsLogin&&MainPlayFab.IsAllDataGet);
            print("Load");
            async.allowSceneActivation = true;
        }
        isLoadClick = true;
    }
}
