using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 该计算器用于实现一连串的复杂计算如:1.25 + 2.5 * 2
/// </summary>

public class ComplexCalculatorUI : MonoBehaviour
{

    public TMP_Text InputText;
    private string _formula;

    private void Awake()
    {
        InputText.text = "";
        _formula = "";
    }
    
    public void OnButtonClick(string value)
    {
        if (value != "=")
        {
            _formula = _formula + value;
        }
        else
        {
            _formula = ComplexCalculatorMgr.Instance.Calculate(_formula).ToString();
        }
        
        InputText.text = _formula;
    }

    public void Clear()
    {
        _formula = "";
        InputText.text = _formula;
    }

    public void OnButtonClickTest()
    {
        string testStr = "10+2*5";
        double result = ComplexCalculatorMgr.Instance.Calculate(testStr);
        Debug.Log($"result {result}");
    }

   
    
}
