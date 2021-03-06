using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// IF系统例子
/// </summary>
public class IFSystemSample : FractalLineBehaviour
{
    private IFSystem system = new IFSystem();
    /// <summary>
    /// 向左旋转角度
    /// </summary>
    public float LeftDegree = 15f;
    /// <summary>
    /// 向右旋转角度
    /// </summary>
    public float RightDegree = -15f;
    /// <summary>
    /// 线段长度
    /// </summary>
    public float Length = 0.1f;

    /// <summary>
    /// 原型
    /// </summary>
    public string Origin = "F";
    /// <summary>
    /// 等价符号
    /// </summary>
    public string GenerateSymbol = "->";
    /// <summary>
    /// 单词集
    /// </summary>
    public string[] Words = new string[] { "F", "+", "-", "[", "]" };
    /// <summary>
    /// 等价规则
    /// </summary>
    public string[] Rules = new string[] { 
        "F->FF+[+F-F-F]-[-F+F+F]",
        "F->F[+F][-F]"
    };
    /// <summary>
    /// 规则概率
    /// </summary>
    public float[] RuleProbablitys = new float[] {
        0.25f,
        0.75f
    };
    /// <summary>
    /// 分形次数
    /// </summary>
    public int FractalCount = 4;

    void Start()
    {
        system.Origin = Origin;
        system.GenerateSymbol = GenerateSymbol;
        system.Words = Words;
        system.Rules.Clear();
        if (Rules != null && Rules.Length > 0 && RuleProbablitys != null && RuleProbablitys.Length == Rules.Length)
        {
            for (int i = 0; i < Rules.Length; i++)
            {
                system.AddRule(RuleProbablitys[i], Rules[i]);
            }
        }
        
        system.ParseHandle += OnParseWord;

        InitFractal(FractalCount);
    }

    protected void InitFractal(int count)
    {
        linePoints.Clear();
        InitCurrentNode();
        string ifsText = system.CreateText(count);
        system.ParseText(ifsText);

        Debug.LogFormat("[IFSystem] fractal text {0}", ifsText);
    }

    /// <summary>
    /// 解析字符串
    /// </summary>
    /// <param name="type"></param>
    private void OnParseWord(string type)
    {
        if (linePoints == null)
        {
            return;
        }

        if (type == "F")
        {
            Vector3 offset = new Vector3(0, Length, 0);
            bool empty = linePoints.Count <= 0;
            if (empty)
            {
                linePoints.Add(new Vector3());
            }
            Vector3 startPoint = linePoints[currentNode.index];
            if (!empty)
            {
                linePoints.Add(startPoint);
            }

            offset = currentNode.matrix * offset;
            Vector3 endPoint = offset + startPoint;
            linePoints.Add(endPoint);

            int curIndex = linePoints.Count - 1;
            currentNode.index = curIndex;
        }
        else if (type == "+")
        {
            currentNode.matrix *= Matrix2x2.Rotate(LeftDegree * Mathf.Deg2Rad);
        }
        else if (type == "-")
        {
            currentNode.matrix *= Matrix2x2.Rotate(RightDegree * Mathf.Deg2Rad);
        }
        else if (type == "[")
        {
            if (linePoints.Count > 0)
            {
                nodeInfos.Push(currentNode);
            }
        }
        else if (type == "]")
        {
            if (nodeInfos.Count > 0)
            {
                currentNode = nodeInfos.Peek();
                nodeInfos.Pop();
            }
        }
    }
}
