using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataManager : SingletonBehaviour<UserDataManager>
{
    public string UserName { get; private set; }

    public void SetUserName(string userName)
    {
        UserName = userName;
    }

}
