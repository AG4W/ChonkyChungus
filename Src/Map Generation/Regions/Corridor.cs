using System.Collections.Generic;

public class Corridor : Region
{
    public Corridor(List<Tile> tiles)
    {
        base.tiles = tiles;

        for (int i = 0; i < base.tiles.Count; i++)
        {
            base.tiles[i].SetIndex(-2);
            base.tiles[i].SetStatus(TileStatus.Vacant);
        }
    }
}
