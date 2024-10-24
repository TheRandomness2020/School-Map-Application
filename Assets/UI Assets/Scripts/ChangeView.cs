using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;

public class ChangeView : MonoBehaviour
{
    public GameObject fromPanel;
    public GameObject toPanel;
    public GlobalControlandVars gVC;

    public void ChangePanel()
    {
        fromPanel.SetActive(false);
        toPanel.SetActive(true);
        gVC.StudentMode = !gVC.StudentMode;
    }
}
