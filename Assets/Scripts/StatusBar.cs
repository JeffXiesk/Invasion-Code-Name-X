using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    public Text MoneyCount;

    public void SetMoney(float money){
        MoneyCount.text = ((int)money).ToString();
    }
}
