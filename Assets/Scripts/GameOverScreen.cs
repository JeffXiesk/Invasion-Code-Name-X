using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public Text StateText;

    public void Setup(string res){
        gameObject.SetActive(true);
        StateText.text = res;
    }
}
