using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void GroundBuiltEvent(GroundBuilder builder);

public class GroundBuilder : MonoBehaviour
{
    public event GroundBuiltEvent OnCompleted;

    [SerializeField] GroundBuilder upstreamGB;
    [SerializeField, Range(5, 100)] float radius;
    [SerializeField, Range(0, 5)] float width;
    [SerializeField, Range(0, 2)] float arcDistanceRatio = 0.2f;
    [SerializeField, Range(0, 1)] float tileBuildTime = 0.2f;
    [SerializeField] GroundTile groundTilePrefab;
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

    private float Width
    {
        get
        {
            return upstreamGB == null ?
                width :
                2 * (Mathf.Abs(upstreamGB.transform.position.z - transform.position.z) - upstreamGB.width / 2);
        }
    }

    private float NoiseDepthOffset
    {
        get
        {
            return  upstreamGB != null ? upstreamGB.NoiseDepthOffset + upstreamGB.lateralNoiseScale + lateralNoiseScale : 0;
        }
    }

    private int DepthDirection
    {
        get
        {
            return upstreamGB == null ? 0 : (upstreamGB.DepthDirection + 1 % 2);
        }
    }

    IEnumerator<WaitForSeconds> Build()
    {
        float zOffset = Origo.ZOffset(transform);
        float noiseDepthOffset = NoiseDepthOffset;
        float depthDirection = DepthDirection;
        bool invertVertsOrder = depthDirection == 1;
        float myWidth = Width;
        int nTiles = Mathf.FloorToInt(2 * Mathf.PI * radius / (myWidth * arcDistanceRatio));
        if (nTiles % 2 == 1) nTiles++;
        float aStep = 2 * Mathf.PI / nTiles;
        float halfWidth = myWidth * 0.5f;
        GroundTile prev = null;
        for (int idTile=0; idTile<nTiles; idTile++)
        {
            float depth = (idTile % 2 == depthDirection ? -1 : 1);
            Vector3 depthOffset = Vector3.forward * depth * halfWidth;
            float noiseDepth = depth * lateralNoiseScale;
            Vector3 firstVert;
            Vector3 secondVert;
            Vector3 thirdVert;
            if (idTile == 0)
            {
                firstVert = Origo.GetPoint(radius + GetNoise(-noiseDepth + noiseDepthOffset, aStep * -1f), aStep * -1f, zOffset) - depthOffset;
                secondVert = Origo.GetPoint(radius + GetNoise(noiseDepth + noiseDepthOffset, aStep * idTile), aStep * idTile, zOffset) + depthOffset;
                thirdVert = Origo.GetPoint(radius + GetNoise(-noiseDepth + noiseDepthOffset, aStep * (idTile + 1f)), aStep * (idTile + 1f), zOffset) - depthOffset;
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
                    thirdVert = Origo.GetPoint(radius + GetNoise(-noiseDepth + noiseDepthOffset, aStep * (idTile + 1f)), aStep * (idTile + 1f), zOffset) - depthOffset;
                }                
            }

            prev = SpawnAt(idTile, new Vector3[] { firstVert, secondVert, thirdVert }, prev, invertVertsOrder);
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
        return Mathf.PerlinNoise(along, lateral) * noise;
    }

    GroundTile SpawnAt(int id, Vector3[] verts, GroundTile prev, bool invertOrder)
    {
        var tile = Instantiate(groundTilePrefab, transform);
        tile.name = string.Format("Tile {0}", id);
        tile.gameObject.layer = LayerMask.NameToLayer("Ground");
        tile.SetVerts(verts, (id % 2 == 1) != invertOrder);
        tile.negNeighbour = prev;
        return tile;
    }
}
