using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    [SerializeField]
    private GameObject loadingImg;

    private Coroutine loadingAnimRoutine;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Instantiate()
    {
        GameObject gameInstance = Instantiate(Resources.Load("Prefabs/GameManager")) as GameObject;
        DontDestroyOnLoad(gameInstance);

        GameObject sceneChangeManager = new GameObject("SceneChangeManager");
        GameObject dataTableManager = new GameObject("DataTableManager");
        GameObject userDataManager = new GameObject("UserDataManager");

        sceneChangeManager.AddComponent<SceneChangeManager>();
        dataTableManager.AddComponent<DataTableManager>();
        userDataManager.AddComponent<UserDataManager>();

        sceneChangeManager.transform.parent = gameInstance.transform;
        dataTableManager.transform.parent = gameInstance.transform;
        userDataManager.transform.parent = gameInstance.transform;
    }

    public void StartLoading()
    {
        loadingImg.transform.parent.gameObject.SetActive(true);
        if (loadingAnimRoutine == null)
        {
            loadingAnimRoutine = StartCoroutine(LoadingAnim());
        }
    }

    public void StopLoading()
    {
        if(loadingAnimRoutine != null)
        {
            StopCoroutine(loadingAnimRoutine);
        }
        loadingImg.transform.parent.gameObject.SetActive(false);
    }

    private IEnumerator LoadingAnim()
    {
        while(true)
        {
            loadingImg.transform.localEulerAngles += Vector3.forward * 1f;
            yield return new WaitForEndOfFrame();
        }
    }

    
}
