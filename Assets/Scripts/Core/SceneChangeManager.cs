using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : SingletonBehaviour<SceneChangeManager>
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Install ()
    {
        GameObject sceneChangeManager = new GameObject("SceneChangeManager");
        sceneChangeManager.AddComponent<SceneChangeManager>();
        DontDestroyOnLoad(sceneChangeManager);
    }
}
