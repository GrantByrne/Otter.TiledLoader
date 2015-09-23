using System.Xml;

namespace Otter.TiledLoader
{
    public class TiledObject
    {
        /// <summary>
        /// The name of the object.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The horizontal position of the object in pixels on the level.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// The vertical position of the object in pixels on the level.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// The width of the object in pixels on the level.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The height of the object in pixels on the level.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The type as defined in the tiled object group
        /// </summary>
        public string @Type { get; private set; }

        public TiledObject(XmlElement xmlElement)
        {
            Name = xmlElement.Attributes["name"].Value;
            X = (int) xmlElement.AttributeFloat("x");
            Y = (int) xmlElement.AttributeFloat("y");
            Width = (int) xmlElement.AttributeFloat("width");
            Height = (int) xmlElement.AttributeFloat("height");
            @Type = xmlElement.Attributes["type"].Value;
        }
    }
}