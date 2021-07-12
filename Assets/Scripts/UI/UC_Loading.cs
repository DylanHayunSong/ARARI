using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UC_Loading : UC_BaseComponent
{
    [SerializeField]
    private RawImage bg;
    [SerializeField]
    private RawImage loadingImg;

    private Coroutine loadingAnimRoutine;

    public override void BindDelegates ()
    {
        bg.gameObject.SetActive(false);
    }

    public void StartLoading()
    {
        bg.gameObject.SetActive(true);
        if (loadingAnimRoutine == null)
        {
            loadingAnimRoutine = StartCoroutine(LoadingAnim());
        }
    }

    public void StopLoading()
    {
        if (loadingAnimRoutine != null)
        {
            StopCoroutine(loadingAnimRoutine);
            loadingAnimRoutine = null;
        }
        bg.gameObject.SetActive(false);
    }

    private IEnumerator LoadingAnim()
    {
        while (true)
        {
            loadingImg.transform.localEulerAngles += Vector3.forward * 1f;
            yield return new WaitForEndOfFrame();
        }
    }
}
