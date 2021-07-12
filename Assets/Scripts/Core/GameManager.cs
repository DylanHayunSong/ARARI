using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    [SerializeField]
    public UP_GlobalPage globalPage;

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
}
