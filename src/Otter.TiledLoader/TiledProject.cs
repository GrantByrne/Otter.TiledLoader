using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Otter.TiledLoader
{
    /// <summary>
    /// Class used for loading .tmx files created in Tiled Map Editor (http://www.mapeditor.org/).
    /// </summary>
    public class TiledProject : TiledBase
    {
        #region Public Properties

        /// <summary>
        /// The current location of the Tiled file on disk.
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// The width of the map in tiles
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The height of the map in tiles
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// The pixel height of a single tile
        /// </summary>
        public int TileHeight { get; }

        /// <summary>
        /// The pixel width of a single tile
        /// </summary>
        public int TileWidth { get; }

        /// <summary>
        /// The width of the map in pixels
        /// </summary>
        public int PixelWidth => Width * TileWidth;

        /// <summary>
        /// The height of the map in pixels
        /// </summary>
        public int PixelHeight => Height * TileHeight;

        /// <summary>
        /// The tilesets that are used to display graphics on the screen.
        /// </summary>
        public List<TiledTileset> TileSets { get; } = new List<TiledTileset>();

        /// <summary>
        /// The layers that compose the Tiled Level.
        /// </summary>
        public List<TiledLayer> Layers { get; } = new List<TiledLayer>();

        public Dictionary<string, int> ColliderTags = new Dictionary<string, int>();

        #endregion

        #region Constructors

        public TiledProject(string source)
        {
            CheckExists(source);

            Source = source;

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(Source);
            var map = xmlDoc.GetElementsByTagName("map")[0];
            Width = map.AttributeInt("width");
            Height = map.AttributeInt("height");
            TileWidth = map.AttributeInt("tilewidth");
            TileHeight = map.AttributeInt("tileheight");

            LoadProperties(map);
            LoadTilesets(xmlDoc);

            var nodes = map.ChildNodes;
            foreach(XmlElement node in nodes)
            {
                var tag = node.Name;

                if(tag == "layer")
                {
                    var layer = new TiledTileLayer(node);
                    Layers.Add(layer);
                }
                else if(tag == "objectgroup")
                {
                    var layer = new TiledObjectGroup(node);
                    Layers.Add(layer);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates an Otter Tilemap from the data loaded from the Tiled Map Editor file.
        /// </summary>
        /// <param name="tileLayer">
        /// The Tiled Map Editor Layer to be turned into an Otter tilemap.
        /// </param>
        /// <returns>
        /// The Otter tilemap representation of the Tiled Map Editor Layer.
        /// </returns>
        public Tilemap CreateTilemap(TiledTileLayer tileLayer)
        {
            var path = GetTilemapPath(TileSets.First().ImageSource);

            CheckExists(path);

            var tilemap = new Tilemap(path, PixelWidth, PixelHeight, TileWidth, TileHeight);
            tilemap.DefaultLayerName = tileLayer.Name;

            var layerName = tileLayer.Name;
            tilemap.AddLayer(layerName, 1);

            for(var x = 0; x < Width; x++)
            {
                for(var y = 0; y < Height; y++)
                {
                    var i = y * Width + x;
                    var gid = (int)tileLayer.Tiles[i].Gid;
                    if(gid > 0)
                    {
                        tilemap.SetTile(x, y, gid - 1, layerName);
                    }
                }

            }

            return tilemap;
        }

        /// <summary>
        /// Creates an Otter scene based on the data in the tiled map file.
        /// </summary>
        /// <param name="scene">
        /// The scene that all of the map data will be added to 
        /// </param>
        public void LoadLevel(Scene scene)
        {
            foreach(var layer in Layers)
            {
                var tileLayer = layer as TiledTileLayer;
                if(tileLayer != null)
                {
                    var entity = new Entity();
                    if(ColliderTags.ContainsKey(tileLayer.Name))
                    {
                        var tag = ColliderTags[tileLayer.Name];
                        var grid = CreateGridCollider(tileLayer, tag);
                        entity.SetCollider(grid);
                    }
                    else
                    {
                        var tilemap = CreateTilemap(tileLayer);
                        entity.SetGraphic(tilemap);
                    }

                    scene.Add(entity);
                }
                else if(layer is TiledObjectGroup)
                {
                    var objectGroup = (TiledObjectGroup)layer;
                    foreach(var @object in objectGroup.Objects)
                    {
                        var entity = CreateEntity(@object);
                        scene.Add(entity);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a grid collider from any TiledTileLayer. Each tile that have an image set it will 
        /// create a collider the size of the tile.
        /// </summary>
        /// <param name="tileLayer">
        /// The tile layer with all the information needed to generate a grid collider.
        /// </param>
        /// <param name="tags">
        /// The tags that will be applied to all of the colliders in the grid collider.
        /// </param>
        /// <returns>
        /// A grid collider with a collider at each position where a tile is set.
        /// </returns>
        public GridCollider CreateGridCollider(TiledTileLayer tileLayer, params int[] tags)
        {
            var grid = new GridCollider(PixelWidth, PixelHeight, TileWidth, TileHeight, tags);

            foreach(var tile in tileLayer.Tiles)
            {
                if(tile.Gid > 0)
                {
                    grid.SetTile(tile.X, tile.Y);
                }
            }

            return grid;
        }

        /// <summary>
        /// Register a collision tag on a grid layer loaded from the oel file.
        /// </summary>
        /// <param name="tag">The tag to use.</param>
        /// <param name="layerName">The layer name that should use the tag.</param>
        public void RegisterTag(int tag, string layerName)
        {
            ColliderTags.Add(layerName, tag);
        }

        /// <summary>
        /// Register a collision tag on a grid layer loaded from the oel file.
        /// </summary>
        /// <param name="tag">The enum tag to use. (Casts to int!)</param>
        /// <param name="layerName">The layer name that should use the tag.</param>
        public void RegisterTag(Enum tag, string layerName)
        {
            RegisterTag(Convert.ToInt32(tag), layerName);
        }

        /// <summary>
        /// Get a list of all the known layer names from the tiled map file.
        /// </summary>
        /// <returns>
        ///  A list of all the known layer names from the tiled map file.
        /// </returns>
        public List<string> GetLayerNames()
        {
            return Layers.Select(l => l.Name).ToList();
        }

        #endregion

        #region Private Methods

        private static Entity CreateEntity(TiledObject @object)
        {
            var entityType = Util.GetTypeFromAllAssemblies(@object.Type);
            var entity = (Entity)Activator.CreateInstance(entityType);

            entity.Name = @object.Name;
            entity.X = @object.X;
            entity.Y = @object.Y;

            return entity;
        }

        private void LoadTilesets(XmlDocument xmlDoc)
        {
            var xmlTilesets = xmlDoc.GetElementsByTagName("tileset");
            foreach(XmlElement tileset in xmlTilesets)
            {
                TileSets.Add(new TiledTileset(tileset));
            }
        }

        private string GetTilemapPath(string relativePath)
        {
            var levelFolder = Path.GetDirectoryName(Source);
            var path = Path.GetFullPath(Path.Combine(levelFolder, relativePath));
            return path;
        }

        private static void CheckExists(string source)
        {
            if(!File.Exists(source))
            {
                var msg = $"Path to {source} could not be found.";
                throw new ArgumentException(msg);
            }
        }

        #endregion
    }
}