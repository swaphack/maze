using UnityEngine;


/// <summary>
/// 分形工具
/// </summary>
public class FractalHelper
{
    /// <summary>
    /// 中心点
    /// </summary>
    /// <param name="m00"></param>
    /// <param name="m10"></param>
    /// <param name="m11"></param>
    /// <param name="m01"></param>
    /// <returns></returns>
    public static Vector3 GetCenterPoint(Vector3 m00, Vector3 m10, Vector3 m11, Vector3 m01)
    {
        Vector3 centerPoint = 0.25f * (m00 + m10 + m11 + m01);

        return centerPoint;
    }
    /// <summary>
    /// 中点
    /// </summary>
    /// <param name="m00"></param>
    /// <param name="m10"></param>
    /// <returns></returns>
    public static Vector3 GetMidpoint(Vector3 m00, Vector3 m10)
    {
        Vector3 midpoint = 0.5f * (m00 + m10);

        return midpoint;
    }

    /// <summary>
    /// 获取值范围
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public static void GetRange(Vector3 start, Vector3 end, out Vector3 min, out Vector3 max)
    {
        min = new Vector3(Mathf.Min(start.x, end.x), Mathf.Min(start.y, end.y), Mathf.Min(start.z, end.z));
        max = new Vector3(Mathf.Max(start.x, end.x), Mathf.Max(start.y, end.y), Mathf.Max(start.z, end.z));
    }

    /// <summary>
    /// 获取随机值
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static Vector3 GetRandomPosition(Vector3 start, Vector3 end)
    {
        Vector3 value = new Vector3();
        value.x = Random.Range(start.x, end.x);
        value.y = Random.Range(start.y, end.y);
        value.z = Random.Range(start.z, end.z);

        return value;
    }
}
