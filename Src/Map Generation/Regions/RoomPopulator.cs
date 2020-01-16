using UnityEngine;

using System.Collections.Generic;
using System.Linq;

public static class RoomPopulator
{
    static Region _region;
    static Room _room;

    static Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.left,
            Vector2Int.down,
            Vector2Int.right,
            Vector2Int.up
        };

    static LootTable _table;

    public static void Instantiate(Region region, LootTable table)
    {
        _region = region;
        _table = table;

        Debug.Assert(_table.loot.Length > 0, "Warning, " + _table.name + " is empty!");

        //base
        InstantiateFloors();
        InstantiateWalls();

        //type specific
        if(_region is Room)
        {
            _room = _region as Room;

            switch (_room.profile)
            {
                case RegionProfile.Entrance:
                    CreateEntrance();
                    break;
                case RegionProfile.Generic:
                    CreateGeneric();
                    break;
                case RegionProfile.Treasure:
                    CreateTreasureRoom();
                    break;
                default:
                    break;
            }

            if(_room.template.pillars.Length > 0)
                CreatePillars();
        }

        //non-structural stuff
        CreateLightsources();
        CreateLoot();
        //CreateDecals();
    }

    static void InstantiateFloors()
    {
        GameObject floor = new GameObject("floor", typeof(MeshFilter), typeof(MeshRenderer));

        //floortiles are just quads, so we can easily merge them and use the material to save some performance
        MeshFilter mf = floor.GetComponent<MeshFilter>();
        MeshRenderer mr = floor.GetComponent<MeshRenderer>();

        MeshFilter[] floorFilters = new MeshFilter[_region.tiles.Count];
        CombineInstance[] floorCombine = new CombineInstance[floorFilters.Length];

        List<MeshFilter> wallFilters = new List<MeshFilter>();

        //create floor
        for (int i = 0; i < _region.tiles.Count; i++)
            floorFilters[i] = Object.Instantiate(_region.template.floors.Random(), _region.tiles[i].position, Quaternion.identity, null).GetComponentInChildren<MeshFilter>();

        for (int i = 0; i < floorFilters.Length; i++)
        {
            floorCombine[i].mesh = floorFilters[i].sharedMesh;
            floorCombine[i].transform = floorFilters[i].transform.localToWorldMatrix;

            Object.Destroy(floorFilters[i].transform.root.gameObject);
        }

        mf.mesh = new Mesh();
        mf.mesh.CombineMeshes(floorCombine, true);
        mf.mesh.RecalculateNormals();

        Color[] colors = new Color[mf.mesh.vertexCount];

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(
                //smoothness noise
                Mathf.RoundToInt(Mathf.PerlinNoise(mf.mesh.vertices[i].x, mf.mesh.vertices[i].z)),
                //tint noise
                Mathf.RoundToInt(Mathf.PerlinNoise(mf.mesh.vertices[i].x, mf.mesh.vertices[i].z)), 
                1, 
                Mathf.RoundToInt(Mathf.PerlinNoise(mf.mesh.vertices[i].x, mf.mesh.vertices[i].z)));
        }

        mf.mesh.SetColors(colors.ToList());
        mr.material = _region.template.floorMaterials.Random();
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.gameObject.isStatic = true;
    }
    static void InstantiateWalls()
    {
        GameObject walls = new GameObject("walls");

        for (int i = 0; i < _region.edges.Count; i++)
        {
            //create walls
            int x = _region.edges[i].x;
            int z = _region.edges[i].z;

            for (int j = 0; j < directions.Length; j++)
            {
                x = _region.edges[i].x + directions[j].x;
                z = _region.edges[i].z + directions[j].y;

                if (x < 0 || x > Grid.size - 1 || z < 0 || z > Grid.size - 1)
                    continue;
                else
                {
                    GameObject g = null;
                    //we're stepping on something that's non-void
                    if (Grid.Get(x, z).index != -1)
                    {
                        //if were stepping on something thats not void and not part of our room
                        //currently just creating doors
                        //will be replaced with a method that takes a continuous line of tiles
                        //creates doors in the middle, and door-neighbours on the sides
                        if (Grid.Get(_region.edges[i].x, _region.edges[i].z).index != Grid.Get(x, z).index)
                            g = Spawn(_region.template.doors.Random(), Grid.Get(x, z), Quaternion.identity, walls.transform);
                        //g = Object.Instantiate(_room.template.doors.Random(), Grid.Get(x, z).position, Quaternion.identity, walls.transform);
                    }
                    else
                        g = Spawn(_region.template.walls.Random(), Grid.Get(x, z), Quaternion.identity, walls.transform);
                    //g = Object.Instantiate(_room.template.walls.Random(), Grid.Get(x, z).position, Quaternion.identity, walls.transform);

                    //wtf happens that neccesitates this check?
                    if (g == null)
                        continue;

                    MeshFilter mf = g.GetComponentInChildren<MeshFilter>();
                    MeshRenderer mr = g.GetComponentInChildren<MeshRenderer>();

                    //randomize vertex alpha color using perlin for vertex blending in materials
                    Color[] colors = new Color[mf.mesh.vertexCount];

                    for (int k = 0; k < colors.Length; k++)
                        colors[k] = new Color(1, 1, 1, Mathf.PerlinNoise(mf.mesh.vertices[k].x, mf.mesh.vertices[k].y));

                    mf.mesh.SetColors(colors.ToList());

                    //will probably initiate meshes with "REPLACE_ME" material
                    //for easier material swapping
                    for (int l = 0; l < mr.materials.Length; l++)
                        if (mr.materials[l].IsKeywordEnabled("_BlendStrength"))
                            mr.materials[l].SetFloat("_BlendStrength", Mathf.Clamp(Synched.Next(_region.template.averageDirtiness - _region.template.dirtinessVariance, _region.template.averageDirtiness + _region.template.averageDirtiness), -1f, 1f));

                    g.transform.LookAt(_region.edges[i].position, Vector3.up);
                }
            }
        }
    }

    //static void CreateDiningArea()
    //{
    //    //get center
    //    Tile c = Grid.Get(_room.centerX, _room.centerZ);

    //    //buffer from edge
    //    int bufferX = _room.centerX - _room.originX - 4;
    //    int bufferZ = _room.centerZ - _room.originZ - 4;

    //    //table alignment direction, so they all place more or less in line
    //    Vector3 tableAlignment = new Vector3(0, Synched.Next(0, 3 + 1) * 90, 0);
    //    bool alignmentIsHorizontal = tableAlignment.y != 90 && tableAlignment.y != 270;

    //    for (int x = _room.centerX - bufferX; x <= _room.centerX + bufferX; x += alignmentIsHorizontal ? 2 : 4)
    //    {
    //        for (int z = _room.centerZ - bufferZ; z <= _room.centerZ + bufferZ; z += alignmentIsHorizontal ? 4 : 2)
    //        {
    //            //add random chance to skip a table
    //            if (Synched.Next(0, 4 + 1) == 4)
    //                continue;

    //            //create table
    //            Tile t = Grid.Get(x, z);

    //            Spawn(_room.template.tables.Random(), t, Quaternion.Euler(0, tableAlignment.y, 0));

    //            for (int chairX = x - 1; chairX <= x + (alignmentIsHorizontal ? 0 : 1); chairX++)
    //            {
    //                for (int chairZ = z - 1; chairZ <= z + (alignmentIsHorizontal ? 1 : 0); chairZ++)
    //                {
    //                    //skip positions, also introduce 25% of chair missing, for more atmosphere
    //                    if (alignmentIsHorizontal && chairZ == z || !alignmentIsHorizontal && chairX == x || Synched.Next(0, 3 + 1) == 3)
    //                        continue;

    //                    Tile ct = Grid.Get(chairX, chairZ);
                        
    //                    Quaternion rotation = Quaternion.LookRotation(t.position - ct.position);
    //                    rotation *= Quaternion.Euler(0, Synched.Next(-15, 15), 0);

    //                    Spawn(_room.template.chairs.Random(), ct, rotation, null);
    //                }
    //            }
    //        }
    //    }
    //}
    //static void CreateKitchen()
    //{

    //}
    //static void CreateLibrary()
    //{
    //    Tile c = Grid.Get(_room.centerX, _room.centerZ);

    //    //buffer from edge
    //    int bufferX = _room.centerX - _room.originX - 3;
    //    int bufferZ = _room.centerZ - _room.originZ - 3;

    //    Vector3 shelfAlignment = new Vector3(0, Synched.Next(0, 3 + 1) * 90, 0);
    //    bool alignmentIsHorizontal = shelfAlignment.y != 90 && shelfAlignment.y != 270;

    //    for (int x = _room.centerX - bufferX; x <= _room.centerX + bufferX; x += alignmentIsHorizontal ? 2 : 4)
    //    {
    //        for (int z = _room.centerZ - bufferZ; z <= _room.centerZ + bufferZ; z += alignmentIsHorizontal ? 4 : 2)
    //        {
    //            //small chance of skipping shelf
    //            if (Synched.Next(0, 4 + 1) == 4)
    //                continue;

    //            //create table
    //            Spawn(_room.template.shelves.Random(), Grid.Get(x, z), Quaternion.Euler(0, shelfAlignment.y, 0));
    //        }
    //    }
    //}
    //static void CreateChapel()
    //{
    //    //make the rostrum first
    //    Tile t = Grid.Get(_room.centerX, _room.centerZ);
    //    Spawn(_room.template.specific.Random(), t, Quaternion.Euler(0, Synched.Next(0, 3 + 1) * 90, 0), null);

    //    int bufferX = _room.centerX - _room.originX - 2;
    //    int bufferZ = _room.centerZ - _room.originZ - 2;

    //    //make pews
    //    for (int x = _room.centerX - bufferX; x <= _room.centerX + bufferX; x += 3)
    //    {
    //        for (int z = _room.centerZ - bufferZ; z <= _room.centerZ + bufferZ; z += 3)
    //        {
    //            if (Mathf.Abs(x) - _room.centerX < 2 && Mathf.Abs(z) - _room.centerZ < 2 || Synched.Next(0, 3 + 1) == 3)
    //                continue;

    //            Tile ct = Grid.Get(x, z);
    //            Spawn(_room.template.benches.Random(), ct, Quaternion.LookRotation(t.position - ct.position), null);
    //        }
    //    }
    //}

    static void CreateEntrance()
    {
        //spawn entrance light
        Spawn(_room.template.specific.Random(), Grid.Get(_room.centerX, _room.centerZ), Quaternion.Euler(0, Synched.Next(0, 3 + 1) * 90, 0));
    }
    static void CreateGeneric()
    {
        if(_room.template.specific.Length > 0)
            Spawn(_room.template.specific.Random(), Grid.Get(_room.centerX, _room.centerZ), Quaternion.Euler(0, Synched.Next(0, 3 + 1) * 90, 0));
    }
    static void CreateTreasureRoom()
    {
        //todo add more stuff here later on
        Spawn(_room.template.specific.Random(), Grid.Get(_room.centerX, _room.centerZ), Quaternion.Euler(0, Synched.Next(0, 3 + 1) * 90, 0));
    }

    static void CreatePillars()
    {
        Tile o = Grid.Get(_room.centerX, _room.centerZ);

        int totalWidth = (_room.centerX - _room.originX) * 2;
        int totalHeight = (_room.centerZ - _room.originZ) * 2;

        //how far in from edge
        int insetX = Synched.Next(2, 3 + 1);
        int insetZ = Synched.Next(2, 3 + 1);

        int width = totalWidth - (insetX * 2);
        int height = totalHeight - (insetZ * 2);

        int stepX = Synched.Next(2, (width / 2) + 1);
        int stepZ = Synched.Next(2, (height / 2) + 1);

        while (width % stepX != 0)
            stepX = Synched.Next(2, (width / 2) + 1);
        while (height % stepZ != 0)
            stepZ = Synched.Next(2, (height / 2) + 1);

        for (int x = _room.originX + insetX; x <= _room.originX + width + insetX; x += stepX)
        {
            for (int z = _room.originZ + insetZ; z <= _room.originZ + height + insetZ; z += stepZ)
            {
                if (x == _room.originX + insetX || x == _room.originX + width + insetX || z == _room.originZ + insetZ || z == _room.originZ + height + insetZ)
                {
                    Tile t = Grid.Get(x, z);

                    if (t.status != TileStatus.Vacant)
                        continue;

                    if (Synched.Next(0f, 1f) <= _room.template.pillarSpawnProbability)
                        Spawn(_room.template.pillars.Random(), t, Quaternion.Euler(0, Synched.Next(0, 3 + 1) * 90, 0));
                    //else if (_region.template.statues.Length > 0 && Synched.Next(0f, 1f) <= _room.template.statueSpawnProbability)
                    //    Spawn(_room.template.statues.Random(), t, Quaternion.Euler(0, Synched.Next(0, 3 + 1) * 90, 0));
                }
            }
        }
    }
    static void CreateLightsources()
    {
        if (_region.template.lightSpawnProbability == 0f)
            return;

        for (int i = 0; i < _region.GetTiles(TileStatus.Vacant).Length; i++)
        {
            if (Synched.Next(0f, 1f) <= _region.template.lightSpawnProbability)
                Spawn(_region.template.lights.Random(), _region.tiles[i], Quaternion.Euler(0f, Synched.Next(0f, 360f), 0f));
        }
    }
    static void CreateLoot()
    {
        if (_region.template.lootSpawnProbability == 0f)
            return;

        for (int i = 0; i < _region.GetTiles(TileStatus.Vacant).Length; i++)
        {
            if(Synched.Next(0f, 1f) <= _region.template.lootSpawnProbability)
                Spawn(_table.loot.Random(), _region.GetTiles(TileStatus.Vacant)[i], Quaternion.Euler(0f, Synched.Next(0f, 360f), 0f));
        }
    }
    static void CreateDecals()
    {
        for (int i = 0; i < _region.tiles.Count; i++)
        {
            if (Synched.Next(0f, 1f) <= _region.template.puddleDensity)
                Object.Instantiate(_region.template.puddles.Random(), _region.GetTiles(TileStatus.Vacant)[i].position, Quaternion.Euler(0f, Synched.Next(0f, 360f), 0f), null);
            //else if (Synched.Next(0f, 1f) <= _region.template.dirtDensity)
            //    Object.Instantiate(_region.template.dirt.Random(), _region.tiles[i].position, Quaternion.Euler(0f, Synched.Next(0f, 360f), 0f), null);
        }
    }

    static GameObject Spawn(GameObject prefab, Tile origin, Quaternion rotation, Transform parent = null)
    {
        GameObject g = Object.Instantiate(prefab, origin.position, rotation);
        PCGEntityData data = g.GetComponent<PCGEntityData>();
        Entity entity = g.GetComponentInChildren<Entity>();

        if(data == null)
        {
            Debug.LogWarning("'" + prefab.name + "' is missing a PCGEntityData-component!", prefab);
            return g;
        }

        if (data.subEntities.Length > 0)
        {
            for (int i = 0; i < data.subEntities.Length; i++)
            {
                if (data.subEntities[i].ignoreSpawnProbability || Synched.Next(0f, 1f) <= _region.template.subEntitiyDensities[(int)data.subEntities[i].type])
                {
                    //spawn random object from list
                    //randomize offset from parameters
                    //randomize rotation from parameters relative to subentity rotation
                    //parent under root, IMPORTANT, subentity placeholder object will be removed
                    GameObject sg = Object.Instantiate(
                        data.subEntities[i].prefabs.Random(),
                        data.subEntities[i].transform.position + new Vector3(
                            Synched.Next(-data.subEntities[i].spawnOffset.x, data.subEntities[i].spawnOffset.x),
                            Synched.Next(-data.subEntities[i].spawnOffset.y, data.subEntities[i].spawnOffset.y),
                            Synched.Next(-data.subEntities[i].spawnOffset.z, data.subEntities[i].spawnOffset.z)),
                        Quaternion.Euler(
                            Synched.Next(0, data.subEntities[i].rotationOffset.x),
                            Synched.Next(0, data.subEntities[i].rotationOffset.y),
                            Synched.Next(0, data.subEntities[i].rotationOffset.z))
                            * data.subEntities[i].transform.rotation,
                        g.transform);

                    //randomize scale - uniformly
                    float scale = Synched.Next(-data.subEntities[i].scaleVariance, data.subEntities[i].scaleVariance);
                    sg.transform.localScale += new Vector3(scale, scale, scale);
                }

                //cleanup junk
                Object.Destroy(data.subEntities[i].gameObject);
            }
        }

        //this is in fucking world space jfc
        //gonna need to transform this relative to rotation of entity somehow
        //... later
        origin.SetStatus(data.blocksTile ? TileStatus.Blocked : TileStatus.Vacant);
        origin.SetBlocksLineOfSight(data.blocksLineOfSight);

        //for (int x = data.negativeSize.x; x <= data.positiveSize.x; x++)
        //{
        //    for (int z = data.negativeSize.y; z <= data.positiveSize.y; z++)
        //    {
        //        Tile t = Grid.Get(origin.x + x, origin.z + z);

        //        if (t != null)
        //        {
        //            t.SetStatus(data.blocksTile ? TileStatus.Blocked : TileStatus.Vacant);
        //            t.SetBlocksLineOfSight(data.blocksLineOfSight);
        //        }
        //    }
        //}

        entity?.SetPosition(origin);

        //cleanup junk
        Object.Destroy(data);
        return g;
    }
}