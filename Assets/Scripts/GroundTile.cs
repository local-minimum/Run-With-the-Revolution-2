using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class GroundTile : MonoBehaviour
{
    public GroundTile posNeighbour { get; set; }

    GroundTile _negNeighbour;
    public GroundTile negNeighbour { 
        get
        {
            return _negNeighbour;
        }

        set
        {            
            _negNeighbour = value;
            if (value != null)
            {
                value.posNeighbour = this;
            }
        }
    }

    Vector3[] _verts;
    bool clockWise;
    public Vector3[] verts {
        get { return _verts; }
    }

    public void SetVerts(Vector3[] verts, bool clockWise)
    {
        _verts = verts;
        this.clockWise = clockWise;
        transform.position = verts.Aggregate((a, b) => a + b) / verts.Length;
        MakeGround();        
    }

    void MakeGround()
    {
        var mesh = new Mesh();
        mesh.name = string.Format("{0} mesh", name);
        mesh.vertices = verts.Select(a => transform.InverseTransformPoint(a)).ToArray();
        mesh.triangles = clockWise ? new int[] { 0, 1, 2} : new int[] { 2, 1, 0 };
        mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
