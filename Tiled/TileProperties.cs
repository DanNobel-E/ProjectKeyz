using System;
using System.Collections.Generic;

namespace AIV_Exam_SimoneSantagati
{
     class TileProperties
    {

        Dictionary<string, object> props;

        public TileProperties()
        {
            props = new Dictionary<string, object>();
        }

        public bool Has(string name)
        {
            return props.ContainsKey(name);
        }



        public bool GetBool(string name)
        {
            return (bool)props[name];
        }



        public void SetBool(string name, bool value)
        {
            props[name] = value;
        }

        public void SetString(string name, string value)
        {
            props[name] = value;
        }

        public string GetString(string name)
        {
            return (string)props[name];
        }
    }
}