using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUICalculator : MonoBehaviour
{
    string str = "";
    string str1 = "";

    double a;
    // Start is called before the first frame update

    private char[,] num = new char[4, 4]
    {
        {'7', '8', '9', '/'},
        {'4', '5', '6', '*'},
        {'1', '2', '3', '-'},
        {'0', '.', '=', '+'}
    };


    void Start()
    {
        // Init();
        // OnGUI();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnGUI()
    {
        GUI.Label(new Rect(85, 20, 100, 30), "简易计算器");
        GUI.Box(new Rect(180, 15, 260, 285), " ");
        string text = GUI.TextField(new Rect(210, 30, 200, 40), " ");

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                // int num = 9-(3*j+i+1);
                if (GUI.Button(new Rect(210 + i * 50, 80 + j * 50, 50, 50), num[j, i].ToString()))
                {
                    GUI.Label(new Rect(25, 25, 100, 30), num[j, i].ToString());
                    str += num[j, i];
                    str1 = str;
                    if (num[j, i] == '=')
                    {
                        Debug.Log(str);

                        if (str.Length > 1)
                        {
                            Debug.Log(Cal(str));
                            a = Cal(str);
                        }

                        str1 = a.ToString();
                        str = "";
                    }
                }
            }
        }

        string text1 = GUI.TextField(new Rect(210, 30, 200, 40), str1);
    }

    double Cal(string str)
    {
        Stack<char> s = new Stack<char>();
        Stack<double> num = new Stack<double>();
        double a, b;
        char ch;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] >= '0' && str[i] <= '9')
            {
                a = 0;
                while (str[i] >= '0' && str[i] <= '9')
                {
                    a = a * 10 + (str[i] - '0');
                    i++;
                }

                if (str[i] == '.')
                {
                    i++;
                    double pow = 0.1;
                    while (str[i] >= '0' && str[i] <= '9')
                    {
                        a = a + pow * (str[i] - '0');
                        pow *= 0.1;
                        i++;
                    }
                }

                i--;
                num.Push(a);
            }

            else if (str[i] == '*')
            {
                b = 0;
                i++;
                while (str[i] >= '0' && str[i] <= '9')
                {
                    b = b * 10 + (str[i] - '0');
                    i++;
                }

                i--;
                a = num.Pop();
                a *= b;
                num.Push(a);
            }

            else if (str[i] == '/')
            {
                b = 0;
                i++;
                while (str[i] >= '0' && str[i] <= '9')
                {
                    b = b * 10 + (str[i] - '0');
                    i++;
                }

                i--;
                a = num.Pop();
                a /= b;
                num.Push(a);
            }

            else if (str[i] == '+' || str[i] == '-')
            {
                if (s.Count > 0)
                {
                    ch = s.Pop();
                    a = num.Pop();
                    b = num.Pop();

                    if (ch == '+')
                    {
                        a += b;
                    }
                    else
                    {
                        a = b - a;
                    }

                    num.Push(a);
                }

                s.Push(str[i]);
            }
        }

        while (s.Count > 0)
        {
            ch = s.Pop();
            a = num.Pop();
            b = num.Pop();

            if (ch == '+')
            {
                a += b;
            }
            else
            {
                a = b - a;
            }

            num.Push(a);
        }

        return num.Peek();
    }
}