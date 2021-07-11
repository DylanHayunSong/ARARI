using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnityGoogleSheet.Load<DataTable.NickName>();

        //List
        foreach (var value in DataTable.NickName.NickNameList)
        {
            Debug.Log(value.index + "," + value.index + "," + value.nickname);
        }

        //Map (Dictionary)
        var dataFromMap = DataTable.NickName.NickNameMap[0];
        Debug.Log("dataFromMap : " + dataFromMap.index + ", " + dataFromMap.nickname);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
