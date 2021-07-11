using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UC_SceneSelectBtn : UC_BaseComponent
{
    [SerializeField]
    private Button btn;
    [SerializeField]
    private Text text;
    [SerializeField, FilePopup("Scenes/SubScenes/*.unity")] 
    private string sceneRef;

    public override void BindDelegates ()
    {
        btn.onClick.AddListener(LoadScene);
    }

    private void LoadScene()
    {
        sceneRef = sceneRef.Replace("Scenes/SubScenes/","");
        sceneRef = sceneRef.Replace(".unity", "");
        SceneManager.LoadScene(sceneRef.ToString(), LoadSceneMode.Single);
    }
}
