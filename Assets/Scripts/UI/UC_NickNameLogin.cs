using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UC_NickNameLogin : UC_BaseComponent
{
    [SerializeField]
    public Text alertText;
    [SerializeField]
    public InputField nickNameInput;
    [SerializeField]
    private Button loginBtn;

    private List<string> remoteNicknameList = new List<string>();

    public override void BindDelegates ()
    {
        loginBtn.onClick.AddListener(OnClick_LoginBtn);
    }

    private void OnClick_LoginBtn ()
    {
        StartCoroutine(NicknameExistCheck(nickNameInput.text));
    }

    private IEnumerator NicknameExistCheck(string nickName)
    {
        loginBtn.interactable = false;

        List<string> existNicknames = new List<string>();

        DataTable.NickName.LoadFromGoogle((list, map) =>
        {
            foreach (var data in list)
            {
                existNicknames.Add(data.nickname);
            }
        }, true);

        GameManager.Instance.StartLoading();
        yield return new WaitWhile(() => existNicknames.Count == 0);
        GameManager.Instance.StopLoading();

        if (existNicknames.Contains(nickName))
        {
            Debug.Log("Success");
            UserDataManager.Instance.SetUserName(nickName);
            SceneManager.LoadScene("SelectScene");
        }
        else
        {
            Debug.Log("Fail");
        }

        loginBtn.interactable = true;
    }
}
