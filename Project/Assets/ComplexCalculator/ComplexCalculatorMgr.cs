using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 该单例提供Calculate接口用于计算复杂的表达式,比如:1.25 + 2.5 * 2
/// </summary>

public class ComplexCalculatorMgr : Singleton<ComplexCalculatorMgr>
{
    /// <summary>
    /// 计算接口
    /// </summary>
    /// <param name="str"></param> example:"10 + 2 * 3"
    /// <returns></returns>
    public double Calculate(string str)
    {
        //用于存储结果,最后进行累加
        Stack<double> stack = new Stack<double>();
        //记录符号位,默认为+
        char preSigh = '+';
        //str中的单个数字
        double num = 0;
        int size = str.Length;
        //遍历字符串
        for (int i = 0; i < size; i++)
        {
            //转为单个数字
            if (char.IsDigit(str[i]) || str[i] == '.')
            {
                while (i < size && char.IsDigit(str[i]))
                {
                    num = num * 10 + str[i] - '0';
                    i++;
                }

                if (i < size && str[i] == '.')
                {
                    i++;
                    double pow = 0.1;
                    while ( i < size && char.IsDigit(str[i]))
                    {
                        num = num + (str[i] - '0') * pow;
                        pow *= 0.1;
                        i++;
                    }
                }
            }

            //防止越界
            if (i >= size)
            {
                i = size - 1;
            }
            
            //判断计算符
            if ((!char.IsDigit(str[i]) && str[i] != '.' && str[i] != ' ') || i == size - 1)
            {
                switch (preSigh)
                {
                    //'+','-'直接把上次的num入栈即可,乘除需要把栈顶的数字和当前的num进行计算
                    case '+':
                        stack.Push(num);
                        break;
                    case '-':
                        stack.Push(-num);
                        break;
                    case '*':
                        stack.Push(stack.Pop() * num);
                        break;
                    case '/':
                        stack.Push(stack.Pop() / num);
                        break;
                    default:
                        Debug.LogError($"wrong operator {preSigh}");
                        break;
                }
                
                //操作完了之后清空num以及更新preSign
                num = 0;
                preSigh = str[i];
            }
        }
        
        double result = 0;
        //把stack里面的数全部做加法即可
        while (stack.Count > 0)
        {
            result += stack.Pop();
        }
        
        return result;
    }
    
 
}
