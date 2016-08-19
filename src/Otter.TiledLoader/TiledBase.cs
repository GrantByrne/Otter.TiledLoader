using System.Collections.Generic;
using System.Xml;

namespace Otter.TiledLoader
{
    public abstract class TiledBase
    {
        /// <summary>
        /// Custom properties that are attached to the tiled element
        /// </summary>
        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Loads the properties associated with the tiled element
        /// </summary>
        /// <param name="xmlElement"></param>
        protected void LoadProperties(XmlNode xmlElement)
        {
            var properties = xmlElement["properties"];

            if (properties == null)
            {
                return;
            }

            foreach (XmlNode child in properties.ChildNodes)
            {
                if (child.Name == "property" && child.Attributes != null && child.Attributes.Count > 0)
                {
                    Properties.Add(child.Attributes["name"].Value, child.Attributes["value"].Value);
                }
            }
        }
    }
}
