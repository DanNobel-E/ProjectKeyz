using System;
using System.Collections.Generic;
using System.Xml;

namespace AIV_Exam_SimoneSantagati
{
     class TmxReader
    {
        public TileSet TileSet { get; }

        public List<Layer> Layers { get; internal set; }

        public TmxReader(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            XmlNode nodeMap = doc.SelectSingleNode("map");


            //Parsing tileset

            TileSet = TmxNodeParser.ParseTileset(nodeMap);

            //Parsing grid

            Layers = TmxNodeParser.ParseLayers(nodeMap, TileSet);



        }


    }
}