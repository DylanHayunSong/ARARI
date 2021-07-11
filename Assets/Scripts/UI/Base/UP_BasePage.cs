using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UP_BasePage : MonoBehaviour
{
    List<UC_BaseComponent> childCompos = new List<UC_BaseComponent>();

    protected virtual void Awake()
    {
        foreach(var elem in GetComponentsInChildren<UC_BaseComponent>())
        {
            childCompos.Add(elem);
            elem.parentPage = this;
            elem.BindDelegates();
        }
    }
}
