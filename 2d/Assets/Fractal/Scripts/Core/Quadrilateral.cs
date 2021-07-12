using UnityEngine;

/// <summary>
/// 四边形
/// 
/// 3  2
/// 0  1
/// </summary>
public class Quadrilateral
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

    public Quadrilateral()
    {
        point0 = point1 = point2 = point3 = new Vector3();
    }

    public Quadrilateral(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        this.SetPoints(p0, p1, p2, p3);
    }
    /// <summary>
    /// 设置顶点
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    public void SetPoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        point0 = p0;
        point1 = p1;
        point2 = p2;
        point3 = p3;
    }
    /// <summary>
    /// 自动分裂
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="quad0"></param>
    /// <param name="quad1"></param>
    /// <param name="quad2"></param>
    /// <param name="quad3"></param>
    public void AutoDivide(Vector3 offset, 
        out Quadrilateral quad0, out Quadrilateral quad1, out Quadrilateral quad2, out Quadrilateral quad3)
    {
        // 3  2
        // 0  1
        Vector3 centerPoint = FractalHelper.GetCenterPoint(point0, point1, point2, point3);

        centerPoint += offset;

        Vector3 midPoint01 = FractalHelper.GetMidpoint(point0, point1);
        Vector3 midPoint12 = FractalHelper.GetMidpoint(point1, point2);
        Vector3 midPoint23 = FractalHelper.GetMidpoint(point2, point3);
        Vector3 midPoint30 = FractalHelper.GetMidpoint(point3, point0);

        quad0 = new Quadrilateral(point0, midPoint01, centerPoint, midPoint30);
        quad1 = new Quadrilateral(midPoint01, point1, midPoint12, centerPoint);
        quad2 = new Quadrilateral(centerPoint, midPoint12, point2, midPoint23);
        quad3 = new Quadrilateral(midPoint30, centerPoint, midPoint23, point3);
    }
}