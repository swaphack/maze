using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Terrian2DSample : MonoBehaviour
{
    private DiamondSquareTerrian terrian = new DiamondSquareTerrian();
    /// <summary>
    /// 宽度
    /// </summary>
    public float Width = 10;
    /// <summary>
    /// 长度
    /// </summary>
    public float Length = 10;
    /// <summary>
    /// 计算偏移值
    /// </summary>
    public Vector3 Offset = new Vector3(1,10,1);

    /// <summary>
    /// 分形次数
    /// </summary>
    public int FractalCount = 4;
    /// <summary>
    /// 是否灰色图片
    /// </summary>
    public bool IsGrayImage = true;


    void Start()
    {
        terrian.point0 = new Vector3(0, 0, 0);
        terrian.point1 = new Vector3(Width, 0, 0);
        terrian.point2 = new Vector3(Width, 0, Length);
        terrian.point3 = new Vector3(0, 0, Length);
        terrian.Offset = Offset;

        InitFractal(FractalCount);
    }

    protected void InitFractal(int count)
    {
        List<Vector3> points = new List<Vector3>();
        List<int> indices = new List<int>();
        List<Color> colorData = new List<Color>();

        int[] aryIndex = new int[] {0,1,2,3 };
        Vector3[] aryPoint = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };

        List<Quadrilateral> ponts = terrian.CreateQuads(count);
        if (ponts != null && ponts.Count >= 2)
        {
            for (int i = 0; i < ponts.Count; i++)
            {
                aryPoint[0] = ponts[i].point0;
                aryPoint[1] = ponts[i].point1;
                aryPoint[2] = ponts[i].point2;
                aryPoint[3] = ponts[i].point3;

                for (int j = 0; j < 4;j++)
                {
                    var value = aryPoint[j];
                    int index = points.IndexOf(value);
                    if (index == -1)
                    {
                        points.Add(value);
                        if (IsGrayImage)
                        {
                            float gray = (value.y + Offset.y) / (2 * Offset.y);
                            colorData.Add(new Color(gray, gray, gray));
                        }
                        else
                        {
                            colorData.Add(new Color(value.x / Width, value.y / Offset.y, value.z / Length));
                        }

                        aryIndex[j] = points.Count - 1;
                    }
                    else
                    {
                        aryIndex[j] = index;
                    }
                }
                // 0, 3, 1, 1, 3, 2
                int[] aryIdx = new int[] { 
                    aryIndex[0], aryIndex[3], aryIndex[1],
                    aryIndex[1], aryIndex[3], aryIndex[2]
                };
                indices.AddRange(aryIdx);
            }
        }

        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.vertices = points.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.colors = colorData.ToArray();
        meshFilter.mesh = mesh;
    }
}
