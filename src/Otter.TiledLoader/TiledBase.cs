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
            if(properties != null)
            {
                var children = xmlElement.ChildNodes;
                foreach(XmlNode child in children)
                {
                    if(child.Name == "properties")
                    {
                        var attributes = child.Attributes;
                        if(attributes != null && attributes.Count > 0)
                        {
                            var name = attributes["name"].Value;
                            var value = attributes["value"].Value;
                            Properties.Add(name, value);
                        }
                    }
                }
            }
        }
    }
}