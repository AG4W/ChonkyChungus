using UnityEngine;

using System.Collections.Generic;
using System.Linq;

public static class RoomPopulator
{
    static PCGTemplate _profile;

    static float _yOffset = .0001f;

    public static void Populate(Room room)
    {
        Load(room.type);

        switch (room.type)
        {
            case RoomType.Entrance:
                CreateEntrance(room);
                CreatePillars(room);
                break;
            case RoomType.Commons:
                CreateDiningArea(room);
                CreatePillars(room);
                break;
            //case RoomType.Kitchen:
            //    CreateKitchen(room);
            //    break;
            case RoomType.Library:
                CreateLibrary(room);
                CreatePillars(room);
                break;
            case RoomType.Chapel:
                CreateChapel(room);
                CreatePillars(room);
                break;
            default:
                break;
        }

        CreateRubble(room);
        CreateLights(room);
        CreateFloorDecals(room);

        CreateLoot(room);
    }
    static void Load(RoomType type)
    {
        _profile = Resources.LoadAll<PCGTemplate>("PCGTemplates/").Where(p => p.type == type).ToArray().Random();
    }

    static void CreateEntrance(Room room)
    {
        Spawn(_profile.specific.Random(), Grid.Get(room.centerX, room.centerZ), Quaternion.identity);
    }
    static void CreateDiningArea(Room room)
    {
        //get center
        Tile c = Grid.Get(room.centerX, room.centerZ);

        //buffer from edge
        int bufferX = room.centerX - room.originX - 4;
        int bufferZ = room.centerZ - room.originZ - 4;

        //table alignment direction, so they all place more or less in line
        Vector3 tableAlignment = new Vector3(0, Synched.Next(0, 3 + 1) * 90, 0);
        bool alignmentIsHorizontal = tableAlignment.y != 90 && tableAlignment.y != 270;

        for (int x = room.centerX - bufferX; x <= room.centerX + bufferX; x += alignmentIsHorizontal ? 2 : 4)
        {
            for (int z = room.centerZ - bufferZ; z <= room.centerZ + bufferZ; z += alignmentIsHorizontal ? 4 : 2)
            {
                //add random chance to skip a table
                if (Synched.Next(0, 4 + 1) == 4)
                    continue;

                //create table
                Tile t = Grid.Get(x, z);

                Spawn(_profile.tables.Random(), t, Quaternion.Euler(0, tableAlignment.y, 0));

                for (int chairX = x - 1; chairX <= x + (alignmentIsHorizontal ? 0 : 1); chairX++)
                {
                    for (int chairZ = z - 1; chairZ <= z + (alignmentIsHorizontal ? 1 : 0); chairZ++)
                    {
                        //skip positions, also introduce 25% of chair missing, for more atmosphere
                        if (alignmentIsHorizontal && chairZ == z || !alignmentIsHorizontal && chairX == x || Synched.Next(0, 3 + 1) == 3)
                            continue;

                        Tile ct = Grid.Get(chairX, chairZ);
                        
                        Quaternion rotation = Quaternion.LookRotation(t.position - ct.position);
                        rotation *= Quaternion.Euler(0, Synched.Next(-15, 15), 0);

                        Spawn(_profile.chairs.Random(), ct, rotation, null);
                    }
                }
            }
        }
    }
    static void CreateKitchen(Room room)
    {

    }
    static void CreateLibrary(Room room)
    {
        Tile c = Grid.Get(room.centerX, room.centerZ);

        //buffer from edge
        int bufferX = room.centerX - room.originX - 3;
        int bufferZ = room.centerZ - room.originZ - 3;

        Vector3 shelfAlignment = new Vector3(0, Synched.Next(0, 3 + 1) * 90, 0);
        bool alignmentIsHorizontal = shelfAlignment.y != 90 && shelfAlignment.y != 270;

        for (int x = room.centerX - bufferX; x <= room.centerX + bufferX; x += alignmentIsHorizontal ? 2 : 4)
        {
            for (int z = room.centerZ - bufferZ; z <= room.centerZ + bufferZ; z += alignmentIsHorizontal ? 4 : 2)
            {
                //small chance of skipping shelf
                if (Synched.Next(0, 4 + 1) == 4)
                    continue;

                //create table
                Spawn(_profile.shelves.Random(), Grid.Get(x, z), Quaternion.Euler(0, shelfAlignment.y, 0));
            }
        }
    }
    static void CreateChapel(Room room)
    {
        //make the rostrum first
        Tile t = Grid.Get(room.centerX, room.centerZ);
        Spawn(_profile.specific.Random(), t, Quaternion.Euler(0, Synched.Next(0, 3 + 1) * 90, 0), null);

        int bufferX = room.centerX - room.originX - 2;
        int bufferZ = room.centerZ - room.originZ - 2;

        //make pews
        for (int x = room.centerX - bufferX; x <= room.centerX + bufferX; x += 3)
        {
            for (int z = room.centerZ - bufferZ; z <= room.centerZ + bufferZ; z += 3)
            {
                if (Mathf.Abs(x) - room.centerX < 2 && Mathf.Abs(z) - room.centerZ < 2 || Synched.Next(0, 3 + 1) == 3)
                    continue;

                Tile ct = Grid.Get(x, z);
                Spawn(_profile.benches.Random(), ct, Quaternion.LookRotation(t.position - ct.position), null);
            }
        }
    }

    static void CreateRubble(Room room)
    {
        for (int i = 0; i <= Mathf.RoundToInt(room.tiles.Count * _profile.rubbleDensity); i++)
        {
            Tile t = room.GetRandom(true);

            //create rubble
            Spawn(_profile.rubble.Random(), t, Quaternion.Euler(0, Synched.Next(0, 360), 0));
            //create gravel decal
            Spawn(_profile.gravel.Random(), t, Quaternion.Euler(0, Synched.Next(0, 360), 0), null, true);
        }
    }
    static void CreatePillars(Room room)
    {
        Tile o = Grid.Get(room.centerX, room.centerZ);

        int totalWidth = (room.centerX - room.originX) * 2;
        int totalHeight = (room.centerZ - room.originZ) * 2;

        //how far in from edge
        int insetX = Synched.Next(1, 3 + 1);
        int insetZ = Synched.Next(1, 3 + 1);

        int width = totalWidth - (insetX * 2);
        int height = totalHeight - (insetZ * 2);

        int stepX = Synched.Next(2, (width / 2) + 1);
        int stepZ = Synched.Next(2, (height / 2) + 1);

        while (width % stepX != 0)
            stepX = Synched.Next(2, (width / 2) + 1);
        while (height % stepZ != 0)
            stepZ = Synched.Next(2, (height / 2) + 1);

        for (int x = room.originX + insetX; x <= room.originX + width + insetX; x += stepX)
        {
            for (int z = room.originZ + insetZ; z <= room.originZ + height + insetZ; z += stepZ)
            {
                if (x == room.originX + insetX || x == room.originX + width + insetX || z == room.originZ + insetZ || z == room.originZ + height + insetZ)
                {
                    Tile t = Grid.Get(x, z);

                    if (!t.isTraversable || Synched.Next(0, 3 + 1) == 3)
                        continue;

                    Spawn(_profile.pillars.Random(), t, Quaternion.identity);
                }
            }
        }
    }
    static void CreateFloorDecals(Room room)
    {
        Queue<Tile> visited = new Queue<Tile>();

        for (int i = 0; i <= Mathf.RoundToInt(room.tiles.Count * _profile.grungeDensity); i++)
        {
            Tile t = room.GetRandom(true);

            while (visited.Contains(t))
                t = room.GetRandom(true);

            Spawn(_profile.grunges.Random(), t, Quaternion.Euler(0, Synched.Next(0, 360), 0), null, true);

            visited.Enqueue(t);
        }
        for (int i = 0; i <= Mathf.RoundToInt(room.tiles.Count * _profile.gravelDensity); i++)
        {
            visited.Clear();
            Tile t = room.GetRandom(true);

            while (visited.Contains(t))
                t = room.GetRandom(true);

            Spawn(_profile.gravel.Random(), t, Quaternion.Euler(0, Synched.Next(0, 360), 0), null, true);

            visited.Enqueue(t);
        }
        for (int i = 0; i <= Mathf.RoundToInt(room.tiles.Count * _profile.miscDensity); i++)
        {
            visited.Clear();
            Tile t = room.GetRandom(true);

            while (visited.Contains(t))
                t = room.GetRandom(true);

            Spawn(_profile.misc.Random(), t, Quaternion.Euler(0, Synched.Next(0, 360), 0), null, true);

            visited.Enqueue(t);
        }
    }
    static void CreateLights(Room room)
    {
        for (int i = 0; i <= Mathf.RoundToInt(room.tiles.Count * _profile.lightDensity); i++)
            Spawn(_profile.lights.Random(), room.GetRandom(true), Quaternion.Euler(0, Synched.Next(0, 360), 0));
    }
    static void CreateLoot(Room room)
    {
        for (int i = 0; i <= Mathf.RoundToInt(room.tiles.Count * _profile.lootDensity); i++)
        {
            GameObject g = Spawn(
                _profile.containers.Random(),
                room.GetRandom(true),
                Quaternion.Euler(0, Synched.Next(0, 360), 0));

            g.GetComponent<ContainerEntity>().Initialize(_profile.lootWhitelist, ItemRarity.Ancient);
        }
    }

    static GameObject Spawn(GameObject prefab, Tile origin, Quaternion rotation, Transform parent = null, bool offsetY = false)
    {
        GameObject g = Object.Instantiate(prefab, offsetY ? origin.position + Vector3.up * _yOffset : origin.position, rotation);
        PCGEntityData data = g.GetComponent<PCGEntityData>();
        Entity entity = g.GetComponentInChildren<Entity>();

        if(data == null)
        {
            Debug.LogWarning(prefab.name + " is missing a PCGEntityData-component!");
            return g;
        }

        if(entity == null)
        {
            if (data.blocksTile)
                origin.SetStatus(TileStatus.Blocked);
        }
        else
            entity.SetPosition(origin);

        _yOffset += .0001f;

        if (_yOffset > .05f)
            _yOffset = .0001f;

        Object.Destroy(data);
        return g;
    }
}