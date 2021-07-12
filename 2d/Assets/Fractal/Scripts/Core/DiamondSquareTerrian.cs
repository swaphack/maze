using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 棱形-正方形
/// </summary>
public class DiamondSquareTerrian
{
    /// <summary>
    /// 左下角
    /// </summary>
    public Vector3 point0 { get; set; }
    /// <summary>
    /// 右下角
    /// </summary>
    public Vector3 point1 { get; set; }
    /// <summary>
    /// 右上角
    /// </summary>
    public Vector3 point2 { get; set; }
    /// <summary>
    /// 左上角
    /// </summary>
    public Vector3 point3 { get; set; }

    public Vector3 Offset { get; set; }

    /// <summary>
    /// 创建四边形
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<Quadrilateral> CreateQuads(int count)
    {
        Vector3 offset0 = Offset;
        Vector3 offset1 = -Offset;

        Vector3 minOffset, maxOffset;
        FractalHelper.GetRange(offset0, offset1, out minOffset, out maxOffset);

        List<Quadrilateral> quads = new List<Quadrilateral>();
        quads.Add(new Quadrilateral(point0, point1, point2, point3));

        Quadrilateral quad0, quad1, quad2, quad3;
        for (int i = 0; i < count; i++)
        {
            float value = 1.0f / Mathf.Pow(2, i);
            offset0 = minOffset * value;
            offset1 = maxOffset * value;

            List<Quadrilateral> newQuads = new List<Quadrilateral>();
            foreach (var item in quads)
            {
                Vector3 offset = FractalHelper.GetRandomPosition(offset0, offset1);
                item.AutoDivide(offset,
                    out quad0, out quad1, out quad2, out quad3);
                newQuads.Add(quad0);
                newQuads.Add(quad1);
                newQuads.Add(quad2);
                newQuads.Add(quad3);
            }
            quads = newQuads;
        }

        return quads;
    }

    private Vector3 GetRandomPosition(Vector3 start, Vector3 end)
    {
        Vector3 value = new Vector3();
        value.x = Random.Range(start.x, end.x);
        value.y = Random.Range(start.y, end.y);
        value.z = Random.Range(start.z, end.z);

        return value;
    }
}
