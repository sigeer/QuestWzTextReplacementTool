using JiebaNet.Segmenter;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using OpenCCNET;
using Serilog;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace WinFormsApp1
{
    internal class ImageContext : IDisposable
    {
        internal WorkContext Context { get; }
        internal Form1 MainForm { get; }
        public ImageContext(WorkContext context, Form1 form, WzImage image)
        {
            Context = context;
            MainForm = form;
            Image = image;
        }

        string _currentNode = null!;
        public string CurrentNode
        {
            get => _currentNode;
            set
            {
                _currentNode = value;

                _currentIndex = GetPendingItemView().IndexOf(_currentNode);
                MainForm.PendingListWin.HandleNodeChange();
                Context.CurrentNode = CurrentNode;
            }
        }

        int _currentIndex;
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;

                var ds = GetPendingItemView();

                while (_currentIndex < 0)
                {
                    _currentIndex += ds.Count;
                }
                while (_currentIndex >= ds.Count)
                {
                    _currentIndex -= ds.Count;
                }
                _currentNode = ds.ElementAt(_currentIndex);
                MainForm.PendingListWin.HandleNodeChange();
                Context.CurrentNode = _currentNode;
            }
        }


        public WzImage Image { get; set; }


        Dictionary<string, PendingItems> _pendingItems = [];
        List<string>? _pendingItemView;
        public List<string> GetPendingItemView()
        {
            return _pendingItemView ??= _pendingItems.Keys.ToList();
        }

        public List<string> GetEffectiveNodes()
        {
            if (Image.Name == ImageUtils.QuestInfo)
            {
                return Image.WzProperties.Select(x => x.Name).ToList(); ;
            }
            else
            {
                if (Context.FinalData.TryGetValue(ImageUtils.QuestInfo, out var questInfo))
                    return questInfo.GetEffectiveNodes();
                else

                    return Context.SourceFile.WzDirectory.GetImageByName(ImageUtils.QuestInfo).WzProperties.Select(x => x.Name).ToList();
            }
        }


        void SetItemProcessed(PendingItems item, bool processed)
        {
            item.Processed = processed;
            MainForm.PendingListWin.ResetView();

        }

        internal bool IsAllProcessed() => _pendingItems.Values.Count == 0 || _pendingItems.Values.All(x => x.Processed);

        public void TagNewItem(WzImageProperty item)
        {
            Image.AddProperty(ApplyQuestItemPropertyToSimplifed(item));

            var data = new PendingItems(PendingType.NewNode, item);
            _pendingItems[item.Name] = data;
            if (Image.Name != ImageUtils.QuestInfo)
            {
                SetItemProcessed(data, true);
            }
            _pendingItemView = null;
        }

        internal void InserNewItem()
        {
            if (_pendingItems.TryGetValue(CurrentNode, out var node))
            {
                Image.RemoveProperty(CurrentNode);
                Image.AddProperty(ApplyQuestItemPropertyToSimplifed(node.Node));


                SetItemProcessed(node, true);
            }

        }

        internal void RemoveNewItem()
        {
            if (_pendingItems.TryGetValue(CurrentNode, out var node))
            {
                Image.RemoveProperty(CurrentNode);
                SetItemProcessed(node, true);
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
            _pendingItemView = null;
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

        internal Dictionary<string, PendingItems> GetValidPendingItems()
        {
            if (Image.Name == ImageUtils.QuestInfo)
                return _pendingItems;
            else
                return _pendingItems.Where(x => GetEffectiveNodes().Contains(x.Key)).ToDictionary();
        }


        public void SetPropertyValue(string fullPath, string value)
        {
            var node = Image.ResolveFullPath(fullPath);
            node.SetValue(value);

            var editingProp = _pendingItems.Values.FirstOrDefault(x => x.DiffSubProps.Any(y => y.FullPath == fullPath));
            if (editingProp != null)
            {
                SetItemProcessed(editingProp, false);
            }
        }

        public void ResolvePendingItem()
        {
            if (_pendingItems.TryGetValue(CurrentNode, out var item))
            {
                SetItemProcessed(item, true);
            }
        }

        /// <summary>
        /// 不覆盖属性
        /// </summary>
        /// <returns></returns>
        public bool IsIgnorePropertyOverwrite()
        {
            return Image.Name.Equals(ImageUtils.Check, StringComparison.OrdinalIgnoreCase) || Image.Name.Equals(ImageUtils.Act, StringComparison.OrdinalIgnoreCase);
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

        static Regex zhReg = new Regex("[\\u4e00-\\u9fa5]");
        public void SetImgPropertyValue(WzImageProperty rootNode, WzImageProperty oldItem, WzImageProperty? newItem, string path)
        {
            var iTag = oldItem.GetFromPath(path);

            var newTag = newItem?.GetFromPath(path);

            if (iTag != null)
            {
                if (newTag == null)
                {
                    Log.Logger.Warning("{Path}：用于更新的文件不包含该节点", iTag.FullPath, iTag.WzValue);

                    TagPendingItem(rootNode, iTag);
                }


                else if (iTag.PropertyType == newTag.PropertyType)
                {

                    if (iTag.PropertyType != WzPropertyType.String)
                    {
                        if (Image.Name == ImageUtils.QuestInfo && iTag.WzValue != newTag.WzValue)
                        {
                            TagPendingItem(rootNode, iTag);
                        }
                        return;
                    }

                    else
                    {
                        var iValue = iTag.GetString();
                        var nValue = newTag.GetString();
                        if (ImageUtils.ZhCompare(iValue, nValue))
                        {
                            return;
                        }

                        var hasZh1 = zhReg.IsMatch(iValue);
                        var hasZh2 = zhReg.IsMatch(nValue);
                        if (hasZh1)
                        {
                            if (hasZh2)
                            {
                                TagPendingItem(rootNode, iTag);
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            if (hasZh2)
                            {
                                iTag.SetValue(newTag.WzValue);
                            }
                            else
                            {
                                return;
                            }
                        }
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
            _pendingItemView?.Clear();
            _pendingItemView = null;
        }

        WzImageProperty ApplyQuestItemPropertyToSimplifed(WzImageProperty s1)
        {
            var data = new WzSubProperty(s1.Name);
            foreach (var prop in s1.WzProperties)
            {
                if (prop.PropertyType == WzPropertyType.SubProperty)
                {
                    data.AddProperty(ApplyQuestItemPropertyToSimplifed(prop));
                }
                else
                {
                    data.AddProperty(SetImgPropertyValueSimplifed(prop));
                }
            }
            return data;
        }

        WzImageProperty SetImgPropertyValueSimplifed(WzImageProperty data)
        {
            if (data.PropertyType == WzPropertyType.String)
            {
                return new WzStringProperty(data.Name, ZhConverter.HantToHans(data.GetString()));
            }
            else
            {
                return data.DeepClone();
            }
        }
    }

}
