// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0
using System;
using System.Xml.Linq;

namespace Love.Tiled
{
    public class TmxImageLayer : ITmxLayer
    {
        public string Name {get; private set;}

        // TODO: Legacy (Tiled Java) attributes (x, y, width, height)
        public int? Width {get; private set;}
        public int? Height {get; private set;}

        public bool Visible {get; private set;}
        public float Opacity {get; private set;}
        public float OffsetX {get; private set;}
        public float OffsetY {get; private set;}

        public TmxImage Image {get; private set;}

        public PropertyDict Properties {get; private set;}

        float? ITmxLayer.OffsetX => OffsetX;
        float? ITmxLayer.OffsetY => OffsetY;

        public TmxImageLayer(XElement xImageLayer, string tmxDir = "")
        {
            Name = (string) xImageLayer.Attribute("name");

            Width = (int?) xImageLayer.Attribute("width");
            Height = (int?) xImageLayer.Attribute("height");
            Visible = (bool?) xImageLayer.Attribute("visible") ?? true;
            Opacity = (float?) xImageLayer.Attribute("opacity") ?? 1.0f;
            OffsetX = (float?) xImageLayer.Attribute("offsetx") ??0.0f;
            OffsetY = (float?) xImageLayer.Attribute("offsety") ??0.0f;

            Image = new TmxImage(xImageLayer.Element("image"), tmxDir);

            Properties = new PropertyDict(xImageLayer.Element("properties"));
        }
    }
}
