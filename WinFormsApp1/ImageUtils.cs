using MapleLib.WzLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace WinFormsApp1
{
    internal class ImageUtils
    {
        public static List<TextProperty> FlatSelectNode(WzImageProperty? node)
        {
            List<TextProperty> all = [];
            FlatSelectNodeCore(all, node);
            return all.OrderBy(x => x.Name).ToList();
        }

        public static void FlatSelectNodeCore(List<TextProperty> all, WzImageProperty? node)
        {
            if (node == null)
            {
                return;
            }
            foreach (var item in node.WzProperties)
            {
                if (item.PropertyType == WzPropertyType.SubProperty)
                {
                    all.Add(new TextProperty(item.PropertyType.ToString(), item.FullPath, ""));
                    FlatSelectNodeCore(all, item);
                }
                else
                {
                    all.Add(new TextProperty(item.PropertyType.ToString(), item.FullPath, item.WzValue?.ToString()));
                }
            }
        }
    }

    public class TextProperty
    {
        public TextProperty(string type, string name, string? value)
        {
            Type = type;
            Name = name;
            Value = value;
        }

        public string Type { get; set; }
        public string Name { get; set; }
        public string? Value { get; set; }
    }
}
