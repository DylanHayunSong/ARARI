using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UP_SelectSceneMain : UP_BasePage
{
    [SerializeField]
    private Text nicknameText;

    protected override void Awake ()
    {
        base.Awake();

        nicknameText.text = string.Format("<size=125>{0}</size>아 반가워!", UserDataManager.Instance.UserName);
    }
}
