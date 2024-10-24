using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CopyDropDown : MonoBehaviour
{
    public GlobalControlandVars gVC;
    void Update()
    {
        GetComponent<TMP_Dropdown>().options = gVC.roomDropDown.options;
    }
}
