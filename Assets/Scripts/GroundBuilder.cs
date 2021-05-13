using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void GroundBuiltEvent(GroundBuilder builder);

public class GroundBuilder : MonoBehaviour
{
    public event GroundBuiltEvent OnCompleted;

    [SerializeField, Range(5, 100)] float radius;
    [SerializeField, Range(0, 5)] float width;
    [SerializeField, Range(0, 2)] float arcDistanceRatio = 0.2f;
    [SerializeField, Range(0, 1)] float tileBuildTime = 0.2f;
    [SerializeField] Material ground;
    [SerializeField, Range(0, 100)] float noise = 0.25f;
    [SerializeField, Range(0, 4)] float lateralNoiseScale = 0.2f;

    List<GroundTile> tiles = new List<GroundTile>();

    public float Radius
    {
        get { return radius; }
    }

    void Start()
    {
        StartCoroutine(Build());
    }

    IEnumerator<WaitForSeconds> Build()
    {        
        int nTiles = Mathf.FloorToInt(2 * Mathf.PI * radius / (width * arcDistanceRatio));
        if (nTiles % 2 == 1) nTiles++;
        float aStep = 2 * Mathf.PI / nTiles;
        float halfWidth = width * 0.5f;
        GroundTile prev = null;
        for (int idTile=0; idTile<nTiles; idTile++)
        {
            float depth = (idTile % 2 == 0 ? -1 : 1);
            Vector3 depthOffset = Vector3.forward * depth * halfWidth;
            Vector3 firstVert;
            Vector3 secondVert;
            Vector3 thirdVert;
            if (idTile == 0)
            {
                firstVert = Origo.GetPoint(radius + GetNoise(-depth, aStep * -1f), aStep * -1f) - depthOffset;
                secondVert = Origo.GetPoint(radius + GetNoise(depth, aStep * idTile), aStep * idTile) + depthOffset;
                thirdVert = Origo.GetPoint(radius + GetNoise(-depth, aStep * (idTile + 1f)), aStep * (idTile + 1f)) - depthOffset;
            } 
            else
            {
                firstVert = prev.verts[1];
                secondVert = prev.verts[2];
                if (idTile == nTiles - 2)
                {
                    thirdVert = tiles[0].verts[0];
                } else if (idTile == nTiles - 1)
                {
                    thirdVert = tiles[0].verts[1];
                }
                else
                {
                    thirdVert = Origo.GetPoint(radius + GetNoise(-depth, aStep * (idTile + 1f)), aStep * (idTile + 1f)) - depthOffset;
                }                
            }

            prev = SpawnAt(idTile, new Vector3[] { firstVert, secondVert, thirdVert }, prev);
            tiles.Add(prev);
            yield return new WaitForSeconds(tileBuildTime);
        }
        tiles[0].negNeighbour = prev;

        OnCompleted?.Invoke(this);
    }

    float GetNoise()
    {
        return Random.Range(-noise, noise);
    }

    float GetNoise(float lateral, float along) 
    {
        return Mathf.PerlinNoise(along, lateral * lateralNoiseScale) * noise;
    }

    GroundTile SpawnAt(int id, Vector3[] verts, GroundTile prev)
    {
        var tileGO = new GameObject(string.Format("Tile {0}", id), typeof(GroundTile));
        tileGO.transform.SetParent(transform);
        var tile = tileGO.GetComponent<GroundTile>();
        tile.SetVerts(verts, id % 2 == 1, ground);
        tile.negNeighbour = prev;
        return tile;
    }
}
