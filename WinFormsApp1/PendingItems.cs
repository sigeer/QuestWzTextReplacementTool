using MapleLib.WzLib;

namespace WinFormsApp1
{
    internal class PendingItems
    {
        public PendingItems(PendingType type, WzImageProperty nodeProperty)
        {
            Type = type;
            Node = nodeProperty;
            DiffSubProps = [];
        }

        public PendingType Type { get; }
        public WzImageProperty Node { get; }

        public HashSet<WzImageProperty> DiffSubProps { get; }
        public bool Processed { get; set; }
    }

    internal enum PendingType
    {
        NewNode,
        // 不支持移除node
        //RemoveNode,
        PropertyChanged,
    }
}
