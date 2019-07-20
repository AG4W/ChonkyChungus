using UnityEngine;

using Object = UnityEngine.Object;
using System.Collections.Generic;

public class Region
{
    public List<Tile> tiles { get; protected set; }
    public List<Tile> edges { get; protected set; }

    public Region()
    {
        this.tiles = new List<Tile>();
        this.edges = new List<Tile>();
    }

    public virtual GameObject Instantiate()
    {
        GameObject tile = Resources.Load<GameObject>("tile");
        GameObject wall = Resources.Load<GameObject>("wall");

        GameObject floor = new GameObject("floor", typeof(MeshFilter), typeof(MeshRenderer));
        GameObject walls = new GameObject("walls", typeof(MeshFilter), typeof(MeshRenderer));

        MeshFilter[] floorFilters = new MeshFilter[tiles.Count];
        CombineInstance[] floorCombine = new CombineInstance[floorFilters.Length];

        List<MeshFilter> wallFilters = new List<MeshFilter>();

        for (int i = 0; i < tiles.Count; i++)
        {
            //create floor
            floorFilters[i] = Object.Instantiate(tile, tiles[i].position, Quaternion.identity, null).GetComponentInChildren<MeshFilter>();

            //create walls
            for (int x = tiles[i].x - 1; x <= tiles[i].x + 1; x++)
            {
                for (int z = tiles[i].z - 1; z <= tiles[i].z + 1; z++)
                {
                    if (x < 0 || x > Grid.size - 1 || z < 0 || z > Grid.size - 1 || Grid.Get(x, z).index != -1)
                        continue;
                    else
                        wallFilters.Add(Object.Instantiate(wall, Grid.Get(x, z).position, Quaternion.identity, null).GetComponentInChildren<MeshFilter>());
                }
            }
        }

        CombineInstance[] wallCombine = new CombineInstance[wallFilters.Count];

        for (int i = 0; i < floorFilters.Length; i++)
        {
            floorCombine[i].mesh = floorFilters[i].sharedMesh;
            floorCombine[i].transform = floorFilters[i].transform.localToWorldMatrix;

            Object.Destroy(floorFilters[i].transform.root.gameObject);
        }
        for (int i = 0; i < wallFilters.Count; i++)
        {
            wallCombine[i].mesh = wallFilters[i].sharedMesh;
            wallCombine[i].transform = wallFilters[i].transform.localToWorldMatrix;

            Object.Destroy(wallFilters[i].transform.root.gameObject);
        }

        floor.GetComponent<MeshFilter>().mesh = new Mesh();
        floor.GetComponent<MeshFilter>().mesh.CombineMeshes(floorCombine);
        floor.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        floor.GetComponent<MeshRenderer>().material = Resources.LoadAll<Material>("Floors/").Random();

        walls.GetComponent<MeshFilter>().mesh = new Mesh();
        walls.GetComponent<MeshFilter>().mesh.CombineMeshes(wallCombine);
        walls.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        walls.GetComponent<MeshRenderer>().material = Resources.LoadAll<Material>("Walls/").Random();

        //InstantiateDecor();
        return floor;
    }

    public Tile GetRandom(bool isTraversable)
    {
        Tile t = tiles.Random();

        while (t.isTraversable != isTraversable)
            t = tiles.Random();

        return t;
    }
}
