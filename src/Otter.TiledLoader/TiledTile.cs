namespace Otter.TiledLoader
{
    /// <summary>
    /// Class representing a tile loaded from Tiled.
    /// </summary>
    public class TiledTile
    {
        /// <summary>
        /// The Horizontal position of the tile. Measured in tiles.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// The vertical position of the tile. Measured in tiles.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// The id associating this tile with a sprite.
        /// </summary>
        public uint Gid { get; }

        public TiledTile(int x, int y, uint gid)
        {
            X = x;
            Y = y;
            Gid = gid;
        }
    }
}