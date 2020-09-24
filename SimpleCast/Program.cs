using System;
using System.Xml;

namespace SimpleCast
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = new Data();
            int i1 = GetData<int>(data);
            double d = GetData<double>(data);
            string s = GetData<string>(data);
            XmlDocument doc = GetData<XmlDocument>(data);

        }

        static T GetData<T>(Data d)
        {
            Type type = typeof(T);
            switch (type)
            {
                case Type intType when intType == typeof(int):
                    return Caster<int, T>.Cast(d.GetInt());
                case Type doubleType when doubleType == typeof(double):
                    return Caster<double, T>.Cast(d.GetDouble());
                case Type stringType when stringType == typeof(string):
                    return Caster<string, T>.Cast(d.GetString());
                case Type xmlType when xmlType == typeof(XmlDocument):
                    return Caster<XmlDocument, T>.Cast(d.GetXml());
                default:
                    throw new NotSupportedException();
            }

        }

        class Data
        {
            public XmlDocument GetXml()
            {
                return new XmlDocument();
            }
            public int GetInt()
            {
                return 2;
            }

            public string GetString()
            {
                return "ss";
            }

            public double GetDouble()
            {
                return 2.0;
            }
        }
    }
}
