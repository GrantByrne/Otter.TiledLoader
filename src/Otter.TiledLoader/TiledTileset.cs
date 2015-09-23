using System.Xml;

namespace Otter.TiledLoader
{
    /// <summary>
    /// Class representing a tileset loaded from Tiled.
    /// </summary>
    public class TiledTileset
    {
        /// <summary>
        /// The name of the tileset.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The path to the tile image on disk.
        /// </summary>
        public string ImageSource { get; }

        /// <summary>
        /// The starting Gid for a tile on the 
        /// </summary>
        public int FirstGid { get; }

        public TiledTileset(XmlElement xTileset)
        {
            Name = xTileset.Attributes["name"].Value;
            FirstGid = xTileset.AttributeInt("firstgid");
            ImageSource = xTileset["image"].Attributes["source"].Value;
        }
    }
}