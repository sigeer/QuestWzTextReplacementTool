using MapleLib.WzLib;
using System.Globalization;
using System.Text;

namespace WinFormsApp1
{
    internal class ImageUtils
    {
        public static List<TextProperty> FlatSelectNode(WzImageProperty? rootNode)
        {
            List<TextProperty> all = [];
            FlatSelectNodeCore(all, rootNode, rootNode);
            return all.OrderBy(x => x.Name).ToList();
        }

        public static void FlatSelectNodeCore(List<TextProperty> all, WzImageProperty? rootNode, WzImageProperty? node)
        {
            if (rootNode == null || node == null)
            {
                return;
            }
            foreach (var item in node.WzProperties)
            {
                if (item.PropertyType == WzPropertyType.SubProperty)
                {
                    all.Add(new TextProperty(item.PropertyType.ToString(), GetPath(rootNode, item), ""));
                    FlatSelectNodeCore(all, rootNode, item);
                }
                else
                {
                    all.Add(new TextProperty(item.PropertyType.ToString(), GetPath(rootNode, item), item.WzValue?.ToString()));
                }
            }
        }

        public static bool ZhCompare(string input1, string input2)
        {
            return NormalizeChineseTextForCompare(input1) == NormalizeChineseTextForCompare(input2);
        }

        static string NormalizeChineseTextForCompare(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return string.Empty;

            // 1️⃣ Unicode 兼容规范化（非常重要）
            s = s.Normalize(NormalizationForm.FormKC);

            var sb = new StringBuilder(s.Length);

            foreach (char c in s)
            {
                // 2️⃣ 忽略所有空白字符
                if (char.IsWhiteSpace(c))
                    continue;

                // 3️⃣ 忽略中英文标点
                var cat = Char.GetUnicodeCategory(c);
                if (cat == UnicodeCategory.OtherPunctuation ||
                    cat == UnicodeCategory.DashPunctuation ||
                    cat == UnicodeCategory.InitialQuotePunctuation ||
                    cat == UnicodeCategory.FinalQuotePunctuation)
                    continue;

                sb.Append(c);
            }

            return sb.ToString();
        }

        static string GetPath(WzImageProperty node, WzImageProperty subNode)
        {
            var prefixNode = node.Parent;
            var prefix = "";
            while (prefixNode != null)
            {
                prefix = Path.Combine(prefixNode.Name, prefix);
                prefixNode = prefixNode.Parent;
            }

            return Path.GetRelativePath(prefix, subNode.FullPath);
        }

        public const string QuestInfo = "QuestInfo.img";
        public const string Say = "Say.img";
        public const string Act = "Act.img";
        public const string Check = "Check.img";
        public static string[] EffectImages = new string[] { QuestInfo, Say, Act, Check };
        public static bool EffectImage(string imgName)
        {
            return EffectImages.Contains(imgName);
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
