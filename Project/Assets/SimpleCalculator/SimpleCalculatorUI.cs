using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 改脚本实现一般的简单计算的功能,即按下一个操作按钮的时候计算结果
/// mac上的计算器还有取反以及百分比这些一元操作符,而且内部逻辑十分复杂,甚至会补一个数(50 + not的结果实际是 50 + -50)
/// 鉴于这是个简单计算器,就不实现这些功能了
/// </summary>

public class SimpleCalculatorUI : MonoBehaviour
{
    public TMP_Text InputText;

    //总的计算公式
    [ShowInInspector] private string _formula;
    //记录当前的计算的结果
    [ShowInInspector]
    private double _result;
    //记录当前输入的数值
    [ShowInInspector]
    private string _currentInputFormula;

    //当前运行的状态
    private enum State
    {
        Idle = 0,                               //默认状态
        InputNumber = 1,                        //输入数字
        InputOperator = 2,                      //输入运算符
        InputNumberAfterInputOperator = 3,      //输入运算符后再输入数字
    };

    [ShowInInspector]
    private State _currentState;

    private Func<string, double> CalculateInterface;

    private void Awake()
    {
        InputText.text = "";
        _formula = "";
        _currentState = State.Idle;
        //CalculateInterface = ComplexCalculatorMgr.Instance.Calculate;
        CalculateInterface = ComplexCalculatorMgrOpt.Instance.Calculate;
        //test
        // string infix = "10 * ( 2 + 3 ) * 4 + 5";
        // string postfix = ComplexCalculatorMgrOpt.Instance.InfixToPostfix(infix);
        // Debug.Log($"infix {infix} postfix {postfix}");
        // Debug.Log($"result {ComplexCalculatorMgrOpt.Instance.CalculatePostfixExpression(postfix)}");
    }

    private void SetState(State newState)
    {
        _currentState = newState;
    }
    
    /// <summary>
    /// 普通数字的点击
    /// </summary>
    /// <param name="value"></param>
    public void OnButtonNumberClick(string value)
    {
        if (_currentState == State.Idle)
        {
            _currentInputFormula = "";
            SetState(State.InputNumber);
        }
        
        if (_currentState == State.InputOperator)
        {
            //输入符号后,再次输入数据变更状态
            SetState(State.InputNumberAfterInputOperator);
            _currentInputFormula = "";
        }
        
        _formula = _formula + value;
        _currentInputFormula = _currentInputFormula + value;
        InputText.text = _currentInputFormula;
    }
    
    /// <summary>
    /// 二元操作符,比如加减乘除的点击
    /// </summary>
    public void OnButtonBinaryOperatorClick(string binaryOperator)
    {
        if (_currentState == State.Idle)
        {
            //没有数组输入的情况下不能进行计算
            return;
        }
        
        if (_currentState == State.InputOperator)
        {
            //不能连续输入运算符
            return;
        }
        
        //只是输入数字后输入运算符号的话,记录当前的输入
        if (_currentState == State.InputNumber)
        {
            _result = CalculateInterface(_formula);
        }
        else if (_currentState == State.InputNumberAfterInputOperator)
        {
            //这时已经可以进行两个数的计算了
            _result = CalculateInterface(_formula);
            _currentInputFormula = _result.ToString();
            _formula = _currentInputFormula;
            InputText.text = _currentInputFormula;
        }
        
        _formula = _formula + binaryOperator;
        SetState(State.InputOperator);
    }

    /// <summary>
    /// 等号的处理
    /// </summary>
    public void EqualOperation()
    {
        if (_currentState == State.InputOperator)
        {
            //输入运算符过后没有输入后面的数字的话,直接显示当前的计算结果,相当于略过此次计算
            _currentInputFormula = _result.ToString();
            _formula = "";
            InputText.text = _currentInputFormula;
            SetState(State.Idle);
            return;
        }
        
        _result = CalculateInterface(_formula);
        _currentInputFormula = _result.ToString();
        _formula = "";
        InputText.text = _currentInputFormula;
        SetState(State.Idle);
    }
    
    /// <summary>
    /// 清空
    /// </summary>
    public void Clear()
    {
        _formula = "";
        _currentInputFormula = "";
        InputText.text = "";
        _result = 0;
        SetState(State.Idle);
    }
    

}
