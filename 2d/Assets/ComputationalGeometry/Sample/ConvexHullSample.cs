using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class ConvexHullSample : MonoBehaviour
{
    /// <summary>
    /// 点个数
    /// </summary>
    public int PointCount = 100;

    [Range(-10, 0)]
    public float minWidth = -1;
    [Range(0, 10)]
    public float maxWidth = 1;

    [Range(-10, 0)]
    public float minHeight = -1;
    [Range(0, 10)]
    public float maxHeight = 1;

    public float delayTime = 1;

    private float _time = 0;

    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();
        Material material = new Material(Shader.Find("Unlit/Color"));
        meshRenderer.material = material;
    }

    // Update is called once per frame
    void Update()
    {
        if (_time > 0)
        {
            _time -= Time.deltaTime;
            return;
        }

        _time = delayTime;

        if (PointCount <= 0)
        {
            return;
        }

        Vector2[] randomPoints = new Vector2[PointCount];

        for (int i = 0; i < PointCount; i++)
        {
            float x = Random.Range(minWidth, maxWidth);
            float y = Random.Range(minHeight, maxHeight);

            randomPoints[i] = new Vector2(x, y);
        }

        Vector2[] outPoint = ConvexHull.GetConvexHull(randomPoints);
        if (outPoint == null || outPoint.Length < 3)
        {
            return;
        }


        int[] triangles = new int[3 * (outPoint.Length - 2)];
        Vector3[] points = new Vector3[outPoint.Length];
        for (int i = 0; i < outPoint.Length; i++)
        {
            points[i] = outPoint[i];
            if (i >= 2)
            {
                triangles[3 * (i - 2)] = 0;
                triangles[3 * (i - 2) + 1] = i - 1;
                triangles[3 * (i - 2) + 2] = i;
            }
        }



        MeshFilter meshFilter = this.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.vertices = points;
        mesh.triangles = triangles;
        meshFilter.mesh = mesh;
    }
}
