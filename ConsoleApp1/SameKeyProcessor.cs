using MapleLib.WzLib;
using Spectre.Console;
using System.Xml.Linq;

namespace ConsoleApp1
{
    /// <summary>
    /// 查找重复键
    /// </summary>
    internal class SameKeyProcessor
    {
        internal static void FindSameKeys()
        {
            var imgPath = AnsiConsole.Prompt(new TextPrompt<string>($"选择文件："));


            var oldDoc = XDocument.Load("old.txt");
            var newDoc = XDocument.Load("new.txt");


            var oldKeys = oldDoc.Element("imgdir").Elements().Select(x => x.Attribute("name").Value).ToList();
            var newKeys = newDoc.Element("imgdir").Elements().Select(x => x.Attribute("name").Value).ToList();

            // 新插入的
            var newInsert = newKeys.Except(oldKeys).ToList();


            using var newImgStream = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
            using var inputImg = new WzImage(imgPath, newImgStream, WzMapleVersion.GMS);

            bool flag = false;

            var e = new WzImage("Say.img");
            List<string> startDuplicateKeys = [];
            for (int i = 0; i < inputImg.WzProperties.Count; i++)
            {
                if (flag)
                {
                    startDuplicateKeys.Add(inputImg.WzProperties[i].Name);
                }
                if (!flag || newInsert.Contains(inputImg.WzProperties[i].Name))
                {
                    e.AddProperty(inputImg.WzProperties[i].DeepClone());
                }

                if (inputImg.WzProperties[i].Name == "1029")
                {
                    flag = true;
                }
            }
            var duplicated = newKeys.Except(newInsert).Count();
            Utils.SaveImg("Say.img", e, WzMapleVersion.GMS);
        }
    }
}
