using System;
using System.Collections.Generic;
using System.Linq;
using Love;

namespace Love.Tiled
{
    public class Renderer
    {
        SpriteBatch spriteBatch;
        TmxMap map;
        //Texture2D tileset;

        int tileWidth;
        int tileHeight;
        int tilesetTilesWide;
        int tilesetTilesHigh;

        public Renderer(string path)
        {
            map = new TmxMap(path);
            //tileset = Content.Load<Texture2D>(map.Tilesets[0].Name.ToString());

            //tileWidth = map.Tilesets[0].TileWidth;
            //tileHeight = map.Tilesets[0].TileHeight;

            //tilesetTilesWide = tileset.Width / tileWidth;
            //tilesetTilesHigh = tileset.Height / tileHeight;
        }

        internal static Image ReadToImage(System.IO.Stream ss)
        {
            return null;
        }

        internal class TileSetResult
        {
            public readonly Image img;
            public readonly RectangleF region;
            public readonly Vector2 drawOffset;

            public TileSetResult(Image img, RectangleF region, Vector2 drawOffset)
            {
                this.img = img;
                this.region = region;
                this.drawOffset = drawOffset;
            }
        }

        internal TileSetResult FindTileSet(int gid)
        {
            TmxTileset targetTileset = null;
            foreach (var item in map.Tilesets)
            {
                if (item.FirstGid <= gid
                    && gid < item.FirstGid + item.TileCount)
                {
                    targetTileset = item;
                    break;
                }
            }

            if (targetTileset != null)
            {
                //var ttt = targetTileset.Tiles[localGid];
                //return ttt.TopLeft;
                //return ttt.Image;
                var localGid = gid - targetTileset.FirstGid;
                //var col = targetTileset.Columns ?? 0; // TODO:fixme
                var iw = targetTileset.Image.ImageData.GetWidth();
                var col = (int)( ((iw - targetTileset.Margin * 2) + targetTileset.Spacing) / (targetTileset.TileWidth + targetTileset.Spacing)); // TODO:fixme
                int yy = localGid / col;
                int xx = localGid - col * yy;
                return new TileSetResult(targetTileset.Image.ImageData,
                    new RectangleF(
                        targetTileset.Margin + (targetTileset.Spacing + targetTileset.TileWidth) * xx - targetTileset.Spacing,
                        targetTileset.Margin + (targetTileset.Spacing + targetTileset.TileHeight) * yy - targetTileset.Spacing,
                        targetTileset.TileWidth, targetTileset.TileHeight
                        ), 
                    new Vector2(targetTileset.TileOffset.X, targetTileset.TileOffset.Y));
            }

            return null;
        }

        public void Draw()
        {
            Draw(map.Layers);
        }

        Vector2[] ToEllipsePoints(Vector2 center, float w, float h, int seg = 30)
        {
            float seg_dt = Mathf.PI * 2 / seg;
            List<Vector2> list = new List<Vector2>();
            for (float rad = 0; rad < Mathf.PI * 2; rad += seg_dt)
            {
                var x = center.X + Mathf.Cos(rad) * w;
                var y = center.Y + Mathf.Sin(rad) * h;
                list.Add(new Vector2(x, y));
            }
            return list.ToArray();
        }

        Vector2 ToPositioOffset(TmxLayerTile tile)
        {
            return new Vector2(tile.X * map.TileWidth, tile.Y * map.TileHeight);
        }

        public void Draw(TmxList<ITmxLayer> layers)
        {
            foreach (var layer in layers)
            {
                if (!layer.Visible)
                    continue;

                if (layer is TmxLayer layerTiledLayer)
                {
                    //Console.WriteLine(layer.ToString() + " :: " + layer.Name);

                    foreach (var tile in layerTiledLayer.Tiles)
                    {
                        //Console.WriteLine($"{tile.Gid}" );
                        var info = FindTileSet(tile.Gid);
                        if (info.img != null)
                        {
                            Graphics.Draw(
                                Graphics.NewQuad(
                                    info.region.X, info.region.Y, info.region.Width, info.region.Height,
                                    info.img.GetWidth(), info.img.GetHeight()),
                                info.img, 
                                
                                info.drawOffset.X + tile.X * map.TileWidth + layer.OffsetX ?? 0,
                                info.drawOffset.Y + tile.Y * map.TileHeight + layer.OffsetY ?? 0
                                );
                        }
                    }


                }
                else if (layer is TmxObjectGroup layerObjGroup)
                {
                    foreach (var tmxObj in layerObjGroup.Objects)
                    {
                        if (!tmxObj.Visible)
                            continue;

                        var ot = tmxObj.ObjectType;
                        if (ot == TmxObjectType.Basic)
                        {
                            Graphics.Rectangle(DrawMode.Fill,
                                tmxObj.X,
                                tmxObj.Y,
                                tmxObj.Width,
                                tmxObj.Height
                                );
                        }
                        else if (ot == TmxObjectType.Ellipse)
                        {
                            var r = new RectangleF(tmxObj.X, tmxObj.Y, tmxObj.Width, tmxObj.Height);
                            Graphics.Polygon(DrawMode.Fill, ToEllipsePoints(
                                r.Center, r.Width / 2f, r.Height /2f, 40
                                ));
                        }
                        else if (ot == TmxObjectType.Polygon)
                        {
                            Graphics.Polygon(DrawMode.Fill, tmxObj.Points.Select(item => new Vector2(item.X, item.Y)).ToArray());
                        }
                        else if (ot == TmxObjectType.Polyline)
                        {
                            Graphics.Line(tmxObj.Points.Select(item => new Vector2(item.X, item.Y)).ToArray());
                        }
                    }

                }
                else if (layer is TmxGroup layerGroup)
                {
                    // group
                    Graphics.Push();
                    Graphics.Translate(layer.OffsetX ?? 0, layer.OffsetY ?? 0);
                    Draw(layerGroup.Layers);
                    Graphics.Pop();
                }
                else if (layer is TmxImageLayer layerImg)
                {
                    // draw img
                    // layerImg.Image;
                    if (layerImg.Image.ImageData != null)
                    {
                        Graphics.Draw(layerImg.Image.ImageData, 
                            layerImg.OffsetX, 
                            layerImg.OffsetY);
                    }

                }
                else
                {
                    // warnning .....
                    Console.WriteLine("unkonw layer");
                }

                //Console.WriteLine(layer.ToString() + " :: " + layer.Name);
            }

        }
    }
}
