using System.Collections.Generic;
using System.Xml;

namespace Otter.TiledLoader
{
    public class TiledObjectGroup : TiledLayer
    {
        /// <summary>
        /// The collection of objects that compose the object group.
        /// </summary>
        public List<TiledObject> Objects { get; } = new List<TiledObject>(); 

        public TiledObjectGroup(XmlElement xmlElement) : base(xmlElement)
        {
            foreach(XmlElement xObject in xmlElement.ChildNodes)
            {
                var o = new TiledObject(xObject);
                Objects.Add(o);
            }
        }
    }
}