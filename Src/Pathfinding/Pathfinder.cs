using UnityEngine;

using PriorityQueue;

using System.Collections.Generic; 
using System;

public static class Pathfinder
{
    public static Dictionary<Tile, float> Dijkstra(Dictionary<Tile, float> map, Tile origin, int maxDistance, bool ignoreOccupiedTiles)
    {
        map.Clear();

        SimplePriorityQueue<Tile, float> open = new SimplePriorityQueue<Tile, float>();
        HashSet<Tile> closed = new HashSet<Tile>();

        open.Enqueue(origin, 0f);
        map.Add(origin, 0f);
    
        while (open.Count > 0)
        {
            Tile t = open.Dequeue();
        
            closed.Add(t);

            float cd = map[t];

            for (int xAround = (int)t.position.x - 1; xAround <= (int)t.position.x + 1; xAround++)
            {
                for (int zAround = (int)t.position.z - 1; zAround <= (int)t.position.z + 1; zAround++)
                {
                    if (xAround < 0 || xAround > Grid.size - 1 || zAround < 0 || zAround > Grid.size - 1)
                        continue;

                    Tile c = Grid.Get(xAround, zAround);

                    if (closed.Contains(c) || (ignoreOccupiedTiles ? !c.isTraversable : c.status != TileStatus.Walkable))
                        continue;

                    float pd = cd + Distance(t, c);
                
                    if (pd > maxDistance)
                        continue;

                    if (map.ContainsKey(c))
                    {
                        if (map[c] > pd)
                            map[c] = pd;

                        open.UpdatePriority(c, pd);
                    }
                    else
                    {
                        map[c] = pd;
                        open.Enqueue(c, pd);
                    }
                }
            }
        }

        return map;
    }
    public static List<Tile> GetPath(Dictionary<Tile, float> map, Tile target, Tile origin)
    {
        List<Tile> path = new List<Tile>();
        HashSet<Tile> visited = new HashSet<Tile>();
    
        Tile current = target;

        while (current != origin)
        {
            path.Add(current);

            Tile closest = null;
            float distance = Mathf.Infinity;

            for (int xAround = (int)current.position.x - 1; xAround <= (int)current.position.x + 1; xAround++)
            {
                for (int zAround = (int)current.position.z - 1; zAround <= (int)current.position.z + 1; zAround++)
                {
                    if (xAround < 0 || xAround > Grid.size - 1 || zAround < 0 || zAround > Grid.size - 1)
                        continue;

                    Tile c = Grid.Get(xAround, zAround);

                    if (!map.ContainsKey(c) || visited.Contains(c))
                        continue;

                    visited.Add(c);
                    float d = map[c];

                    if (closest == null || d < distance)
                    {
                        closest = c;
                        distance = d;
                    }
                }
            }

            if (closest == null)
                return path;
            else
                current = closest;
        }

        path.Reverse();
        return path;
    }
    /// <summary>
    /// Warning, performs a complete dijkstra fill, this is expensive.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public static List<Tile> GetPathExpensive(Tile target, Tile origin, int maxDistance, bool ignoreOccupiedTiles)
    {
        return GetPath(Dijkstra(new Dictionary<Tile, float>(), origin, maxDistance, ignoreOccupiedTiles), target, origin);
    }

    public static Dictionary<Tile, float> LineOfSight(Dictionary<Tile, float> map, Tile origin, int maxDistance)
    {
        map.Clear();

        if (maxDistance == 0)
            throw new Exception("Can't divide by zero, stupid");

        int offset = maxDistance / 2;

        for (int x = origin.x - offset; x <= origin.x + offset; x++)
        {
            for (int z = origin.z - offset; z <= origin.z + offset; z++)
            {
                if (x < 0 || x > Grid.size || z < 0 || z > Grid.size || Distance(origin, Grid.Get(x, z)) > maxDistance)
                    continue;

                List<Tile> tiles = Linecast(origin, Grid.Get(x, z), true);

                for (int i = 0; i < tiles.Count; i++)
                    if (!map.ContainsKey(tiles[i]))
                        map.Add(tiles[i], 1f);
            }
        }

        return map;
    }

    //Bresenham
    public static List<Tile> Linecast(Tile start, Tile end, bool stopAtLineOfSightBlocker = false)
    {
        List<Tile> tiles = new List<Tile>();

        int x = start.x;
        int z = start.z;

        int width = end.x - start.x;
        int height = end.z - start.z;

        Vector2 ds = Vector2.zero;
        Vector2 de = Vector2.zero;

        if (width < 0)
        {
            ds.x = -1;
            de.x = -1;
        }
        else if (width > 0)
        {
            ds.x = 1;
            de.x = 1;
        }

        if (height < 0)
            ds.y = -1;
        else if (height > 0)
            ds.y = 1;

        int longest = Mathf.Abs(width);
        int shortest = Mathf.Abs(height);

        if (longest <= shortest)
        {
            longest = Mathf.Abs(height);
            shortest = Mathf.Abs(width);

            if (height < 0)
                de.y = -1;
            else if (height > 0)
                de.y = 1;

            de.x = 0;
        }

        int numerator = longest >> 1;

        for (int i = 0; i <= longest; i++)
        {
            if (x < 0 || x > Grid.size || z < 0 || z > Grid.size)
                break;

            if (stopAtLineOfSightBlocker && Grid.Get(x, z).blocksLineOfSight)
                break;

            tiles.Add(Grid.Get(x, z));
            numerator += shortest;

            if(!(numerator < longest))
            {
                numerator -= longest;
                x += (int)ds.x;
                z += (int)ds.y;
            }
            else
            {
                x += (int)de.x;
                z += (int)de.y;
            }
        }

        return tiles;
    }

    public static float Distance(Tile a, Tile b)
    {
        return (a.position - b.position).magnitude;
        //return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.z - b.z));
    }
}
