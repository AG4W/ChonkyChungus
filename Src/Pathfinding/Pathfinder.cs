using UnityEngine;
using PriorityQueue;

using System.Collections.Generic;

public static class Pathfinder
{
    public static Dictionary<Tile, float> Dijkstra(Dictionary<Tile, float> map, Tile origin, int maxDistance)
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

                    if (closed.Contains(c) || !c.isTraversable)
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
    public static List<Tile> GetPath(Tile target, Tile origin, int maxDistance)
    {
        return GetPath(Dijkstra(new Dictionary<Tile, float>(), origin, maxDistance), target, origin);
    }

    public static float Distance(Tile a, Tile b)
    {
        return (a.position - b.position).magnitude;
        //return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.z - b.z));
    }
}
