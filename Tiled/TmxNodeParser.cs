using System;
using System.Xml;
using System.Collections.Generic;

namespace AIV_Exam_SimoneSantagati
{
     class TmxNodeParser
    {
        public static TileSet ParseTileset(XmlNode nodeMap)
        {
            XmlNode tilesetNode = nodeMap.SelectSingleNode("tileset");
            int tileW = int.Parse(tilesetNode.Attributes.GetNamedItem("tilewidth").InnerText);
            int tileH = int.Parse(tilesetNode.Attributes.GetNamedItem("tileheight").InnerText);
            XmlNode tImageNode = tilesetNode.SelectSingleNode("image");
            string tImgPath = "Assets/" + tImageNode.Attributes.GetNamedItem("source").InnerText;
            int tImageW = int.Parse(tImageNode.Attributes.GetNamedItem("width").InnerText);
            int tImageH = int.Parse(tImageNode.Attributes.GetNamedItem("height").InnerText);

            TileSet result= TileSetFactory.Create(tileH, tileW, tImgPath, tImageW, tImageH);


            XmlNodeList tileNodes = tilesetNode.SelectNodes("tile");

            Dictionary<uint, TileProperties> propsPerTile = ParseTilesetProperties(tileNodes);
            foreach(uint tileId in propsPerTile.Keys)
            {
                
                TileType t = result.At(tileId);
                t.Props = propsPerTile[tileId];
            }
           

                return result;
        }


        private static Dictionary<uint, TileProperties> ParseTilesetProperties(XmlNodeList tileNodes)
        {
            Dictionary<uint, TileProperties> result = new Dictionary<uint, TileProperties>();
            foreach(XmlNode tileNode in tileNodes)
            {
                uint id = uint.Parse(tileNode.Attributes.GetNamedItem("id").InnerText);
                XmlNodeList propNodes = tileNode.SelectNodes("properties/property");


                result[id] = ParseProperties(propNodes);
            }

            return result;
        }

        private static TileProperties ParseProperties(XmlNodeList propNodes)
        {
            TileProperties props = new TileProperties();
            foreach (XmlNode propNode in propNodes)
            {
                string name = propNode.Attributes.GetNamedItem("name").InnerText;
                string value = propNode.Attributes.GetNamedItem("value").InnerText;
                XmlNode typeNode = propNode.Attributes.GetNamedItem("type");
                string type = "string";

                if(typeNode != null)
                {
                    type = typeNode.InnerText;
                }
                if (type.Equals("bool"))
                {
                    props.SetBool(name, bool.Parse(value));
                }else if (type.Equals("string"))
                {
                   
                    props.SetString(name, value);

                }
            }

            return props;
        }

        public static Layer ParseLayer(XmlNode layerNode, TileSet ts)
        {
            string name = layerNode.Attributes.GetNamedItem("name").InnerText;

            int layerCols = int.Parse(layerNode.Attributes.GetNamedItem("width").InnerText);
            int layerRows = int.Parse(layerNode.Attributes.GetNamedItem("height").InnerText);
            XmlNode dataNode = layerNode.SelectSingleNode("data");
            string csvString = dataNode.InnerText;

            string csv = csvString.Replace("\r\n", "").Replace("\n", "");

            TileGrid tg = TileGridFactory.Create(layerRows, layerCols, csv, ts);

            Layer result = new Layer(name,tg);

            XmlNodeList propNodes = layerNode.SelectNodes("properties/property");
            result.Props = ParseProperties(propNodes);

            return result;
        }

        public static List<Layer> ParseLayers(XmlNode nodeMap, TileSet ts)
        {
            List<Layer> result = new List<Layer>();
            XmlNodeList layerNodes = nodeMap.SelectNodes("layer");

            foreach (XmlNode each in layerNodes)
            {
                result.Add(ParseLayer(each,ts));
            }

            return result;
        }
    }
}