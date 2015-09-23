using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace Otter.TiledLoader
{
    public class TiledTileLayer : TiledLayer
    {
        /// <summary>
        /// The width of the layer in tiles.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The height of the layer in tiles.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// The encoding format that the tiled information is stored as.
        /// </summary>
        public string Encoding { get; }

        public float Opacity { get; } = 1;

        /// <summary>
        /// The tiles that compose the layer
        /// </summary>
        public List<TiledTile> Tiles { get; } = new List<TiledTile>();

        public TiledTileLayer(XmlElement xLayer) : base(xLayer)
        {
            Width = xLayer.AttributeInt("width");
            Height = xLayer.AttributeInt("height");
            Encoding = xLayer["data"].Attributes["encoding"].Value;

            if(xLayer.Attributes["opacity"] != null)
            {
                Opacity = xLayer.AttributeFloat("opacity");
            }
            
            DecodeTiles(xLayer);
        }

        private void DecodeTiles(XmlElement xLayer)
        {
            var xData = xLayer["data"];

            if(Encoding == "base64")
            {
                DecodeBase64Tiles(xData);
            }
            else if(Encoding == "csv")
            {
                DecodeCsvTiles(xData);
            }
            else if(Encoding == null)
            {
                DecodeXmlTiles(xData);
            }
            else
                throw new Exception("TmxLayer: Unknown encoding.");
        }

        private void DecodeBase64Tiles(XmlElement xData)
        {
            var stream = GetTmxBase64DataStream(xData);

            using(var br = new BinaryReader(stream))
                for(int j = 0; j < Height; j++)
                    for(int i = 0; i < Width; i++)
                        Tiles.Add(new TiledTile(i, j, br.ReadUInt32()));
        }

        private void DecodeCsvTiles(XmlElement xData)
        {
            var csvData = xData.Value;
            int k = 0;
            foreach(var s in csvData.Split(','))
            {
                var gid = uint.Parse(s.Trim());
                var x = k % Width;
                var y = k / Width;
                Tiles.Add(new TiledTile(x, y, gid));
                k++;
            }
        }

        private void DecodeXmlTiles(XmlElement xData)
        {
            int k = 0;
            foreach(XmlElement e in xData.GetElementsByTagName("tile"))
            {
                var gid = (uint)e.AttributeInt("gid");
                var x = k % Width;
                var y = k / Width;
                Tiles.Add(new TiledTile(x, y, gid));
                k++;
            }
        }

        private Stream GetTmxBase64DataStream(XmlElement xData)
        {
            if(Encoding != "base64")
                throw new DataException("Only Base64-encoded data is supported.");

            var rawData = Convert.FromBase64String(xData.InnerText);
            Stream data = new MemoryStream(rawData, false);

            var compression = xData.Attributes["compression"].Value;
            if(compression == "gzip")
                data = new GZipStream(data, CompressionMode.Decompress, false);
            else if(compression == "zlib")
            {
                throw new NotSupportedException("Sorry, I didn't want to import a library for this to work. Try using gzip. :(");
            }
            else if(compression != null)
                throw new DataException("Unknown compression.");

            return data;
        }
    }
}