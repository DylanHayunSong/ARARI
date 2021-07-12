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

        GameManager.Instance.globalPage.uc_Loading.StartLoading();
        yield return new WaitWhile(() => existNicknames.Count == 0);
        GameManager.Instance.globalPage.uc_Loading.StopLoading();

        if (existNicknames.Contains(nickName))
        {
            UserDataManager.Instance.SetUserName(nickName);
            SceneManager.LoadScene("SelectScene");
            GameManager.Instance.globalPage.uc_toastText.ToastOn("로그인 성공!");
        }
        else
        {
            GameManager.Instance.globalPage.uc_toastText.ToastOn("로그인 실패 \n 닉네임을 확인해주세요");
        }

        loginBtn.interactable = true;
    }
}
