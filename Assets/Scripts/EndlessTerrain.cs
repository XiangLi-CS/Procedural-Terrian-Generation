using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    //const float scale = 5f;

    public LODInfo[] detailLevels;
    public static float maxViewDistance;       //How far the player can see
    
    public Transform player;
    public Material mapMaterial;

    public static Vector2 playerPosition;       //player position
    static MapGenerator mapGenerator;
    int chunkSize;
    int chunkVisibleInViewDistance;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();


    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();

        maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistance;
        chunkSize = MapGenerator.mapChunkSize - 1;      //the number of chunk size
        chunkVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize);     //the number of chunks that are visible in view distance

    }

    void Update()
    {
        playerPosition = new Vector2(player.position.x, player.position.z) / mapGenerator.terrainData.uniformScale;
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks()
    {
        //Disvisible the last update chunk
        for(int i=0; i<terrainChunksVisibleLastUpdate.Count; i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();


        // the current coordinate of chunk that the player standing on
        int currentChunkCoordX = Mathf.RoundToInt(playerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(playerPosition.y / chunkSize);

        for(int yOffset = -chunkVisibleInViewDistance; yOffset <= chunkVisibleInViewDistance; yOffset++)
        {
            for (int xOffset = -chunkVisibleInViewDistance; xOffset <= chunkVisibleInViewDistance; xOffset++)
            {
                Vector2 viewdChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                //Create new update visible chunk
                if (terrainChunkDictionary.ContainsKey(viewdChunkCoord))
                {
                    terrainChunkDictionary[viewdChunkCoord].UpdateTerrainChunk();
                    if (terrainChunkDictionary[viewdChunkCoord].isVisible())
                    {
                        terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewdChunkCoord]);
                    }
                }
                else
                {
                    terrainChunkDictionary.Add(viewdChunkCoord, new TerrainChunk(viewdChunkCoord, chunkSize, detailLevels, transform, mapMaterial));
                }
            }
        }

    }

    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        //MapData mapData;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        LODInfo[] detailLevels;
        LODMesh[] lodMeshes;

        MapData mapData;
        bool mapDataReceived;
        int previousLODIndex = -1;

        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material)
        {
            this.detailLevels = detailLevels;
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);      //Find the closest point to another point
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObject.transform.position = positionV3 * mapGenerator.terrainData.uniformScale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * mapGenerator.terrainData.uniformScale;
            SetVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];
            for(int i = 0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].LOD);
            }

            mapGenerator.RequestMapData(position, OnMapDataReceived);
        }

        //Receive MapData
        void OnMapDataReceived(MapData mapData)
        {
            //mapGenerator.RequestMeshData(mapData, OnMeshDataReceived);
            this.mapData = mapData;
            mapDataReceived = true;

            //Add texture
/*            Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colourMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture = texture;*/
        }

        //Receive MeshData
        /*void OnMeshDataReceived(MeshData meshData)
        {
            meshFilter.mesh = meshData.CreateMesh();
        }*/


        public void UpdateTerrainChunk()
        {
            if(mapDataReceived)
            {
                float playerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(playerPosition));
                bool visible = playerDstFromNearestEdge <= maxViewDistance;

                if (visible)
                {
                    int lodIndex = 0;

                    for(int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if(playerDstFromNearestEdge > detailLevels[i].visibleDistance)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if(lodIndex != previousLODIndex)
                    {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if (lodMesh.hasReceivedMesh)
                        {
                            previousLODIndex = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                        } else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(mapData);
                        }
                    }
                }

                SetVisible(visible);

            }
            
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool isVisible()
        {
            return meshObject.activeSelf;
        }
    }

    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasReceivedMesh;
        int lod;

        public LODMesh(int lod)
        {
            this.lod = lod;
        }

        void OnMeshDataReceived(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            hasReceivedMesh = true;
        }

        public void RequestMesh(MapData mapData)
        {
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
        }


    }

    [System.Serializable]
    public struct LODInfo
    {
        public int LOD;
        public float visibleDistance;       //In which distance the lod will be actived
    }


}
