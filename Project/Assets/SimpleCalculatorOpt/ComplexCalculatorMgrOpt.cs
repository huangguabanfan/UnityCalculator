using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ComplexCalculatorMgr的优化版本,使用后缀表达式并将计算方法封装
/// </summary>

public class ComplexCalculatorMgrOpt : Singleton<ComplexCalculatorMgrOpt>
{
    /// <summary>
    /// 使用策略模式和工厂模式优化
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public double Calculate(string str)
    {
        //将str转为后缀表达式然后进行计算
        string postfix = InfixToPostfix(str);
        double result = CalculatePostfixExpression(postfix);
        Debug.Log($"Calculate infix {str} postfix {postfix} result {result}");
        return result;
    }

    /// <summary>
    /// 计算后缀表达式的值 10 2 3 + * 4 * 5 +  (infix: 10 * ( 2 + 3 ) * 4 + 5 )
    /// </summary>
    public double CalculatePostfixExpression(string str)
    {
        if (str.Length <= 0)
        {
            return 0;
        }
        
        Stack<double> stack = new Stack<double>();
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] is ' ' or ',')
            {
                //跳过分割符
                continue;
            }
            
            if (char.IsDigit(str[i]) || str[i] == '.')
            {
                //找到一个数字字符后,然后往后找,直到组装完一个完整的数字
                double number = 0;
                while (i < str.Length && char.IsDigit(str[i]))
                {
                    number = number * 10 + (str[i] - '0');
                    i++;
                }
                
                if (i < str.Length && str[i] == '.')
                {
                    double pow = 0.1;
                    i++;
                    while (i < str.Length && char.IsDigit(str[i]))
                    {
                        number = number + (str[i] - '0') * pow;
                        pow = pow * 0.1;
                        i++;
                    }
                }

                //避免多加一次i
                i--;
                //把结果压入栈中
                stack.Push(number);
            }
            else if (isOperator(str[i]))
            {
                //如果是运算符的话可以直接取出两个元素进行操作,前提是该后缀表达式一定是符合要求的
                //注意顺序
                double num2 = stack.Pop();
                double num1 = stack.Pop();
                double res = CalculateNumbers(num1, num2, str[i]);
                //压入栈中,等待后续计算
                stack.Push(res);
            }
        }
        
        //栈顶即为最后的计算结果
        return stack.Peek();
    }

    /// <summary>
    /// 通过给定的运算符计算两个值的结果
    /// </summary>
    private double CalculateNumbers(double operand1, double operand2, char operation)
    {
        OperatorBase op = GetOperator(operation);
        return op.Compute(operand1, operand2);
    }

    /// <summary>
    /// 前缀表达式转化为后缀表达式
    /// </summary>
    /// <param name="str"></param> "10 * ( 2 + 3 ) * 4 + 5"
    /// <returns></returns>         10 2 3 + * 4 * 5 +
    public string InfixToPostfix(string str)
    {
        Stack<char> stack = new Stack<char>();
        string expression = "";
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == ' ' || str[i] == ',')
            {
                //规范显示
                expression += ' ';
            }
            else if (isOperand(str[i]) || str[i] == '.')
            {
                //往后直接找,组合一个数添加进去
                while (i < str.Length && (isOperand(str[i]) || str[i] == '.'))
                {
                    expression += str[i];
                    i++;
                }

                //减一个,别影响后面逻辑
                i--;
                //最后添加一个分割符
                expression += ' ';
            }
            else if (isOperator(str[i]))
            {
                //如果是操作符的话需要进行后续的复杂判断
                //如果stack里面还有其他的操作符,那么就需要比较下优先级
                //如果遇到了比自己优先级更高的符号,那么可以把栈顶的操作符加入到表达式里面,不然还需要继续遍历去结合优先级更高的部分
                //如果栈顶是'('符号的话需要继续遍历后面的直到找到')',因为'('后面的内容是一个整体
                while (stack.Count > 0 && stack.Peek() != '(' && HasHigherPrecedence(stack.Peek(), str[i]))
                {
                    //把栈顶运算符加入到表达式,然后弹出
                    expression += stack.Pop();
                }
                
                //将新的运算符加入到stack中,等待后续的优先级比较
                stack.Push(str[i]);
            }
            else if (str[i] == '(')
            {
                //遇到左括号的时候,加入进stack里面用作后续判断
                stack.Push(str[i]);
            }
            else if (str[i] == ')')
            {
                //遇到有括号的话就需要把整个括号内的内容(运算符)进行结算,将内容全部加入到表达式即可
                //如果括号内有更加复杂的运算比如(2 + 3 * 2),那么括号里面的内容处理交给上面的if
                //走到这里相当于只有单一的符号了,相当于把括号里面的内容结算并且消除括号
                while (stack.Count > 0 && stack.Peek() != '(')
                {
                    expression += stack.Pop();
                }
                
                //走到这里相当于只剩下'(',直接弹出
                stack.Pop();
            }
        }
        
        //将剩下的运算符直接加到表达式即可
        while (stack.Count > 0)
        {
            expression += stack.Pop();
        }

        return expression;
    }

    bool isOperand(char c)
    {
        //直接使用API也行
        //return char.IsDigit(c);
        return c is >= '0' and <= '9';
    }

    bool isOperator(char c)
    {
        return c is '+' or '-' or '*' or '/';
    }

    bool HasHigherPrecedence(char op1, char op2)
    {
        return GetOperatorWeight(op1) >= GetOperatorWeight(op2);
    }

    /// <summary>
    /// 获得操作符的优先级
    /// </summary>
    int GetOperatorWeight(char op)
    {
        int weight = -1;
        if (op is '+' or '-')  weight = 1;
        else if (op is '*' or '/')  weight = 2;

        return weight;
    }

    /// <summary>
    /// 获取具体的运算方法实例
    /// </summary>
    private OperatorBase GetOperator(char calculateOperator)
    {
        OperatorFactory factory;
        switch (calculateOperator)
        {
            //'+','-'直接把上次的num入栈即可,乘除需要把栈顶的数字和当前的num进行计算
            case '+':
                factory = new OperatorAdditionFactory();
                break;
            case '-':
                factory = new OperatorSubTractFactory();
                break;
            case '*':
                factory = new OperatorMultiplyFactory();
                break;
            case '/':
                factory = new OperatorDivisionFactory();
                break;
            default:
                factory = new OperatorAdditionFactory();
                Debug.LogError($"wrong operator {calculateOperator}");
                break;
        }  

        return factory.CreateOperator();
    }
}
