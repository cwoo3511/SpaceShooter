using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void OnClickStartBtn()
    {
        Debug.Log("Click Start Button");
    }

    public void OnClickOptionBtn(string msg)
    {
        Debug.Log("Click Option Button : " + msg);
    }
}
