using System.Collections.Generic;
using UnityEngine;

public class Terrian1DSample : DrawLineBehaviour
{
    private MidpointTerrian terrian = new MidpointTerrian();

    public Vector3 Src = new Vector3(0, 0, 0);
    public Vector3 Dest = new Vector3(10, 0, 0);
    public Vector3 OffsetPosition = new Vector3(10, 0, 0);

    /// <summary>
    /// 分形次数
    /// </summary>
    public int FractalCount = 4;

    void Start()
    {
        terrian.Src = Src;
        terrian.Dest = Dest;
        terrian.OffsetPosition = OffsetPosition;

        InitFractal(FractalCount);
    }

    protected void InitFractal(int count)
    {
        linePoints.Clear();
        List<Vector3> ponts = terrian.CreatePoints(count);
        if (ponts != null && ponts.Count >= 2 )
        {
            for (int i = 0; i < ponts.Count - 1; i++)
            {
                linePoints.Add(ponts[i]);
                linePoints.Add(ponts[i + 1]);
            }
        }
    }
}
