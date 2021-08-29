using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ConeDisplayer : MonoBehaviour
{
    public int nPoints = 20;
    public float coneAngle = 70.0f;
    public float length = 10.0f;


    private void Start()
    {
        MeshFilter filter = GetComponent<MeshFilter>();

        int vertexCount = nPoints + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; ++i)
        {
            float singleRayAngle = coneAngle / nPoints;
            float angle = -coneAngle / 2 + singleRayAngle * i;
            vertices[i + 1] = Quaternion.Euler(0, angle, 0) * Vector3.forward * length;

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }

            Debug.DrawRay(transform.position, vertices[i + 1], Color.red);
        }

        Mesh mesh = new Mesh();
        filter.mesh = mesh;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

}
