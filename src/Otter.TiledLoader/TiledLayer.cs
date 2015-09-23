using System.Xml;

namespace Otter.TiledLoader
{
    public abstract class TiledLayer : TiledBase
    {
        /// <summary>
        /// The name of the layer.
        /// </summary>
        public string Name { get; }

        protected TiledLayer(XmlElement xLayer)
        {
            Name = xLayer.Attributes["name"].Value;

            LoadProperties(xLayer);
        }
    }
}