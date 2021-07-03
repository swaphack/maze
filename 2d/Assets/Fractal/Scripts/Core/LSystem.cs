using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// L 系统
/// </summary>
public class LSystem
{
    /// <summary>
    /// 字符事件
    /// </summary>
    public delegate void WordHandle(string text);
    /// <summary>
    /// 初始状态
    /// </summary>
    public string Origin { get; set; }
    /// <summary>
    /// 字母
    /// </summary>
    public string[] Words { get; set; }
    /// <summary>
    /// 生成规则
    /// </summary>
    public string Rule { get; set; }
    /// <summary>
    /// 产生符号
    /// </summary>
    public string GenerateSymbol { get; set; }
    /// <summary>
    /// 字符处理
    /// </summary>
    public event WordHandle ParseHandle;

    public LSystem()
    {
    }

   /// <summary>
   /// 创建文本
   /// </summary>
   /// <param name="count"></param>
   /// <returns></returns>
    public string CreateText(int count)
    {
        if (string.IsNullOrEmpty(Origin) || string.IsNullOrEmpty(Rule)
            || string.IsNullOrEmpty(GenerateSymbol)
            || Words == null || Words.Length == 0)
        {
            return null;
        }

        string key = GenerateSymbol;

        string[] ruleAry = Rule.Split(new string[] { key }, StringSplitOptions.None);
        if (ruleAry == null || ruleAry.Length != 2)
        {
            return null;
        }

        string a = ruleAry[0];
        string b = ruleAry[1];

        string text = Origin;
        for (int i = 0; i < count; i++)
        {
            text = text.Replace(a, b);
        }

        return text;
    }

    /// <summary>
    /// 解析文本
    /// </summary>
    /// <param name="text"></param>
    public void ParseText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }
        if (Words == null || Words.Length == 0)
        {
            return;
        }
        Dictionary<int, List<string>> keys = new Dictionary<int, List<string>>();
        foreach (var item in Words)
        {
            if (!string.IsNullOrEmpty(item))
            {
                if (!keys.ContainsKey(item.Length))
                {
                    keys.Add(item.Length, new List<string>());
                }
                keys[item.Length].Add(item);
            }
        }

        int cursor = 0;
        do
        {
            bool bFind = false;
            foreach (var item in keys)
            {
                if (cursor + item.Key > text.Length)
                {
                    continue;
                }
                string value = text.Substring(cursor, item.Key);
                if (item.Value.Contains(value))
                {
                    if (ParseHandle != null)
                    {
                        ParseHandle(value);
                    }
                    cursor += item.Key;
                    bFind = true;
                    break;
                }
            }

            if (!bFind)
            {
                break;
            }

        } while (cursor < text.Length);
    }
}
