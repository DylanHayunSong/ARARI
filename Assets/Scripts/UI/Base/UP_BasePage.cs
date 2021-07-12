using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UP_BasePage : MonoBehaviour
{
    protected virtual void Awake()
    {
        foreach(var elem in GetComponentsInChildren<UC_BaseComponent>())
        {
            elem.parentPage = this;
            elem.BindDelegates();
        }
    }
}
