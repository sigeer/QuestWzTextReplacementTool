using MapleLib.WzLib;
using Serilog;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public class ImageContext : IDisposable
    {
        internal Form1 MainForm { get; }
        public ImageContext(Form1 form, WzImage image)
        {
            MainForm = form;
            Image = image;
        }

        public WzImage Image { get; set; }


        Dictionary<string, PendingItems> _pendingItems = [];

        public bool HasItem(string nodeName)
        {
            return _pendingItems.ContainsKey(nodeName);
        }

        public void TagNewItem(WzImageProperty item)
        {
            _pendingItems[item.Name] = new(PendingType.NewNode, item);

        }

        internal void InserNewItem(string nodeName)
        {
            if (_pendingItems.TryGetValue(nodeName, out var node))
            {
                Image.RemoveProperty(nodeName);
                Image.AddProperty(node.Node);
                node.Processed = false;
            }

        }

        internal void RemoveNewItem(string nodeName)
        {
            if (_pendingItems.TryGetValue(nodeName, out var node))
            {
                Image.RemoveProperty(nodeName);
                node.Processed = false;
            }

        }

        public void TagPendingItem(WzImageProperty item, WzImageProperty subProp)
        {
            if (_pendingItems.TryGetValue(item.Name, out var data))
            {
                data.DiffSubProps.Add(subProp);
            }
            else
            {
                _pendingItems[item.Name] = data = new PendingItems(PendingType.PropertyChanged, item);
                data.DiffSubProps.Add(subProp);
            }
        }

        public int GetPendingIndex(string item) => Array.IndexOf(_pendingItems.Keys.ToArray(), item);

        public bool TryGetPendingItemsByIndex(int index, out string? value)
        {
            value = null;
            if (_pendingItems.Count == 0)
            {
                return false;
            }
            while (index < 0)
            {
                index += _pendingItems.Count;
            }
            while (index >= _pendingItems.Count)
            {
                index -= _pendingItems.Count;
            }

            value = _pendingItems.Keys.ElementAt(index);
            return true;
        }

        internal Dictionary<string, PendingItems> GetAllPendingItems()
        {
            return _pendingItems;
        }


        public void SetPropertyValue(string path, string value)
        {
            var node = Image.GetFromPath(path.Replace("\\", "/"));
            node.SetValue(value);

            if (_pendingItems.TryGetValue(node.FullPath, out var n))
            {
                n.Processed = false;
                MainForm.PendingListWin.ReloadData();
            }
        }

        public void HandlePendingItem(string name)
        {
            if (_pendingItems.TryGetValue(name, out var item))
            {
                item.Processed = true;
                MainForm.PendingListWin.ReloadData();
            }
        }

        /// <summary>
        /// 不覆盖属性
        /// </summary>
        /// <returns></returns>
        public bool IsIgnorePropertyOverwrite()
        {
            return Image.Name.Equals("Check.img", StringComparison.OrdinalIgnoreCase) || Image.Name.Equals("Act.img", StringComparison.OrdinalIgnoreCase);
        }

        public void ApplyQuestItemProperty(WzImageProperty rootNode, WzImageProperty s1, WzImageProperty? s2)
        {
            foreach (var prop in s1.WzProperties)
            {
                if (prop.PropertyType == WzPropertyType.SubProperty)
                {
                    ApplyQuestItemProperty(rootNode, prop, s2?.GetFromPath(prop.Name));
                }
                else if (prop.PropertyType == WzPropertyType.String)
                {
                    SetImgPropertyValue(rootNode, s1, s2, prop.Name);
                }
            }
        }

        public void SetImgPropertyValue(WzImageProperty rootNode, WzImageProperty oldItem, WzImageProperty? newItem, string path)
        {
            var iTag = oldItem.GetFromPath(path);

            var newTag = newItem?.GetFromPath(path);

            if (iTag != null)
            {
                if (iTag.PropertyType != WzPropertyType.String)
                {
                    Log.Logger.Verbose("{Path}：当前更新的节点不是String节点，跳过", iTag.FullPath);
                    return;
                }

                if (newTag == null)
                {
                    Log.Logger.Warning("{Path}：用于更新的文件不包含该节点", iTag.FullPath, iTag.WzValue);

                    TagPendingItem(rootNode, iTag);
                }

                else if (iTag.PropertyType == newTag.PropertyType)
                {
                    var isOldHasLetter = iTag.GetString().Any(x => !char.IsDigit(x));
                    var isNewHasLetter = newTag.GetString().Any(x => !char.IsDigit(x));

                    if (isOldHasLetter ^ isNewHasLetter)
                    {
                        TagPendingItem(rootNode, iTag);
                    }
                    else
                    {
                        iTag.SetValue(newTag.WzValue);
                    }
                }

                else
                {
                    TagPendingItem(rootNode, iTag);
                }
            }
            else
            {
                if (newTag == null)
                    return;
                else
                {
                    Log.Logger.Verbose("{Path}：用于更新的文件额外包含这个节点，跳过", newTag.FullPath);
                }
            }
        }
        public void Dispose()
        {
            Image.Dispose();
            _pendingItems.Clear();
        }


    }
}
