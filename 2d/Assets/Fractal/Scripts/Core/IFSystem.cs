using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 函数迭代系统
/// </summary>
public class IFSystem
{
    public class RuleInfo
    {
        /// <summary>
        /// 概率
        /// </summary>
        public float Probability { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        public override string ToString()
        {
            string text = "Rule Info,";
            text += "Content :" + Content;

            return text;
        }
    }
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
    public Dictionary<string, List<RuleInfo>> Rules { get; }
    /// <summary>
    /// 产生符号
    /// </summary>
    public string GenerateSymbol { get; set; }
    /// <summary>
    /// 字符处理
    /// </summary>
    public event WordHandle ParseHandle;

    public IFSystem()
    {
        Rules = new Dictionary<string, List<RuleInfo>>();
    }

    /// <summary>
    /// 添加规则
    /// </summary>
    /// <param name="probability"></param>
    /// <param name="content"></param>
    public void AddRule(float probability, string content)
    {
        if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(GenerateSymbol))
        {
            return;
        }
        string[] ruleAry = content.Split(new string[] { GenerateSymbol }, StringSplitOptions.None);
        if (ruleAry == null || ruleAry.Length != 2)
        {
            return;
        }
        string key = ruleAry[0];
        string value = ruleAry[1];

        if (!Rules.ContainsKey(key))
        {
            Rules.Add(key, new List<RuleInfo>());
        }
        var ruleInfo = new RuleInfo() { Probability = probability, Content = content, Key = key, Value = value };
        Rules[key].Add(ruleInfo);
    }
    /// <summary>
    /// 选择一条规则
    /// </summary>
    /// <returns></returns>
    public string GetRandomRule(string key)
    {
        if (Rules.Count == 0)
        {
            return null;
        }

        List<RuleInfo> values = new List<RuleInfo>();
        if (!Rules.TryGetValue(key, out values))
        {
            return null;
        }

        float totalProbability = 0;
        // 获取全部概率
        foreach(var item in values)
        {
            if (string.IsNullOrEmpty(item.Content))
            {
                continue;
            }
            totalProbability += item.Probability;
        }

        float value = UnityEngine.Random.Range(0, totalProbability);

        float temp = 0;
        foreach (var item in values)
        {
            // 闭区间
            if (value >= temp && value < temp + item.Probability)
            {
                return item.Value;
            }
            temp += item.Probability;
        }

        return null;
    }

    /// <summary>
    /// 创建文本
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public string CreateText(int count)
    {
        if (string.IsNullOrEmpty(Origin)
            || Rules == null || Rules.Count == 0
            || string.IsNullOrEmpty(GenerateSymbol)
            || Words == null || Words.Length == 0)
        {
            return null;
        }
        // 按照key长度分组
        Dictionary<int, List<string>> keys = new Dictionary<int, List<string>>();
        foreach (var item in Rules.Keys)
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

        string text = Origin;
        for (int i = 0; i < count; i++)
        {
            text = CreateRoundText(text, keys);
        }

        return text;
    }

    /// <summary>
    /// 生成一回合的文本
    /// </summary>
    /// <param name="text"></param>
    /// <param name="keys"></param>
    /// <returns></returns>
    protected string CreateRoundText(string text, Dictionary<int, List<string>> keys)
    {
        if (string.IsNullOrEmpty(text) || keys == null || keys.Count == 0)
        {
            return null;
        }
        int cursor = 0;
        do
        {
            bool bFind = false;
            foreach (var item in keys)
            {
                if (cursor + item.Key > text.Length)
                {// 长度超过
                    continue;
                }
                string subText = text.Substring(cursor, item.Key);
                if (item.Value.Contains(subText))
                {// 包含关键字
                    // 获取随机生成规则
                    string ruleValue = GetRandomRule(subText);
                    if (!string.IsNullOrEmpty(ruleValue))
                    {
                        text = text.Remove(cursor, item.Key);
                        text = text.Insert(cursor, ruleValue);
                        cursor += ruleValue.Length;
                        bFind = true;
                        break;
                    }
                }
            }
            if (!bFind)
            {
                cursor++;
            }

        } while (cursor < text.Length);

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
                    ParseHandle?.Invoke(value);
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
