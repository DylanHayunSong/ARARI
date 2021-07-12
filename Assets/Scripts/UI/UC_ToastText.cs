using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UC_ToastText : UC_BaseComponent
{
    [SerializeField]
    private Image bgImg;
    [SerializeField]
    private Text text;

    private Coroutine toastAnimRoutine = null;

    public override void BindDelegates ()
    {
        bgImg.gameObject.SetActive(false);
    }

    public void ToastOn(string toastString)
    {
        if(toastAnimRoutine != null)
        {
            StopCoroutine(toastAnimRoutine);
        }
        text.text = toastString;
        toastAnimRoutine = StartCoroutine(ToastAnim());
    }

    private IEnumerator ToastAnim()
    {
        bgImg.gameObject.SetActive(true);
        Color bgImgColor = bgImg.color;
        bgImgColor.a = 0;
        bgImg.color = bgImgColor;

        Color textColor = text.color;
        textColor.a = 0;
        text.color = textColor;
        float time = 0;

        while(time < GameConstants.ToastAppearTime)
        {
            float animT = Mathf.Lerp(0, 1, time / GameConstants.ToastAppearTime);

            bgImgColor.a = animT;
            bgImg.color = bgImgColor;

            textColor.a = animT;
            text.color = textColor;

            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }

        yield return new WaitForSeconds(GameConstants.ToastTime);

        time = 0;

        while (time < GameConstants.ToastDisappearTime)
        {
            float animT = Mathf.Lerp(1, 0, time / GameConstants.ToastDisappearTime);

            bgImgColor.a = animT;
            bgImg.color = bgImgColor;
            
            textColor.a = animT;
            text.color = textColor;

            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }
        bgImg.gameObject.SetActive(false);

        toastAnimRoutine = null;
    }

}
