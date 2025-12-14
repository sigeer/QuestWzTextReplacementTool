using MapleLib.WzLib;
using Serilog;
using WeifenLuo.WinFormsUI.Docking;

namespace WinFormsApp1
{
    internal class WorkContext
    {
        public const string QuestInfo = "QuestInfo.img";
        internal static WorkContext? Instance { get; set; }
        internal Form1 MainForm { get; }
        public WorkContext(Form1 form, WzFile sourceFile)
        {
            MainForm = form;
            SourceFile = sourceFile;

            AllNodes = SourceFile.WzDirectory.GetImageByName(QuestInfo).WzProperties.Select(x => x.Name).ToList();
            CurrentNode = AllNodes[0];
        }

        public EventHandler<string>? OnNodeChanged { get; set; }

        string _currentNode = null!;
        public string CurrentNode { get => _currentNode; 
            set 
            { 
                _currentNode = value;

                _currentIndex = AllNodes.FindIndex(x => x == _currentNode);
                MainForm.PendingListWin.HandleNodeChange();
            } 
        }

        int _currentIndex;
        public int CurrentIndex { 
            get => _currentIndex; 
            set
            {
                while (_currentIndex < 0)
                {
                    _currentIndex += WorkContext.Instance!.AllNodes.Count;
                }
                while (_currentIndex >= WorkContext.Instance!.AllNodes.Count)
                {
                    _currentIndex -= WorkContext.Instance!.AllNodes.Count;
                }
                _currentNode = AllNodes[_currentIndex];
                MainForm.PendingListWin.HandleNodeChange();
            } 
        }
        public List<string> AllNodes { get; private set; }
        public WzFile SourceFile { get; }

        public Dictionary<string, WzImage?> NewData = [];
        public Dictionary<string, ImageContext> FinalData = [];

        public void SetNewData(WzFile file)
        {
            foreach (var item in NewData)
            {
                item.Value?.Dispose();
            }


            foreach (var item in file.WzDirectory.WzImages)
            {
                NewData[item.Name]?.Dispose();
                NewData[item.Name] = item;

                FinalData[item.Name]?.Dispose();
                FinalData[item.Name] = new ImageContext(
                    MainForm,
                    SourceFile.WzDirectory.GetImageByName(item.Name).DeepClone());

                ApplyQuestImage(FinalData[file.Name]!, NewData[file.Name]!);
            }
        }

        public void SetNewData(WzImage file)
        {
            NewData.GetValueOrDefault(file.Name)?.Dispose();
            FinalData.GetValueOrDefault(file.Name)?.Dispose();

            NewData[file.Name] = file;
            FinalData[file.Name] = new ImageContext(
                MainForm,
                SourceFile.WzDirectory.GetImageByName(file.Name).DeepClone());

            if (file.Name == QuestInfo)
            {
                AllNodes = file.WzProperties.Select(x => x.Name).ToList();
            }

            ApplyQuestImage(FinalData[file.Name]!, NewData[file.Name]!);
        }

        void ApplyQuestImage(ImageContext context, WzImage newData)
        {
            foreach (var item in AllNodes)
            {
                var targetItem = context.Image[item];
                var newItem = newData[item]?.DeepClone();

                if (targetItem == null && newItem != null)
                {
                    context.TagNewItem(newItem);
                }
                else if (targetItem != null)
                {
                    if (newItem != null)
                    {
                        if (!context.IsIgnorePropertyOverwrite())
                        {
                            context.ApplyQuestItemProperty(targetItem, targetItem, newItem);
                        }
                    }
                }
            }
        }

        public bool CheckComplete()
        {
            var sayData = FinalData.GetValueOrDefault("Say.img")?.Image.WzProperties.Select(x => x.Name) ?? [];
            var actData = FinalData.GetValueOrDefault("Act.img")?.Image.WzProperties.Select(x => x.Name) ?? [];
            var checkData = FinalData.GetValueOrDefault("Check.img")?.Image.WzProperties.Select(x => x.Name) ?? [];

            var sayMiss = AllNodes.Except(sayData);
            var actMiss = AllNodes.Except(actData);
            var checkMiss = AllNodes.Except(checkData);

            bool isCompleted = true;
            if (sayMiss.Count() != 0)
            {
                Log.Logger.Warning("{Image} 缺少 {Items}", "Say.img", string.Join(',', sayMiss));
                isCompleted = false;
            }

            if (actMiss.Count() != 0)
            {
                Log.Logger.Warning("{Image} 缺少 {Items}", "Act.img", string.Join(',', actMiss));
                isCompleted = false;
            }

            if (checkMiss.Count() != 0)
            {
                Log.Logger.Warning("{Image} 缺少 {Items}", "Check.img", string.Join(',', checkMiss));
                isCompleted = false;
            }

            return isCompleted;
        }
    }
}
