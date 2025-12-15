using MapleLib.WzLib;
using Serilog;

namespace WinFormsApp1
{
    internal class WorkContext: IDisposable
    {


        internal static WorkContext? Instance { get; set; }
        internal Form1 MainForm { get; }
        public WorkContext(Form1 form, WzFile sourceFile)
        {
            MainForm = form;
            SourceFile = sourceFile;

            AllNodes = SourceFile.WzDirectory.GetImageByName(ImageUtils.QuestInfo).WzProperties.Select(x => x.Name).ToList();
            CurrentNode = AllNodes[0];
        }


        string _currentNode = null!;
        public string CurrentNode
        {
            get => _currentNode;
            set
            {
                _currentNode = value;

                _currentIndex = AllNodes.FindIndex(x => x == _currentNode);
                MainForm.PendingListWin.HandleNodeChange();
            }
        }

        int _currentIndex;
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
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
            foreach (var item in file.WzDirectory.WzImages)
            {
                SetNewData(item);
            }
        }

        public void SetNewData(WzImage file)
        {
            if (!ImageUtils.EffectImage(file.Name))
            {
                return;
            }

            NewData.GetValueOrDefault(file.Name)?.Dispose();
            FinalData.GetValueOrDefault(file.Name)?.Dispose();

            NewData[file.Name] = file;
            FinalData[file.Name] = new ImageContext(
                MainForm,
                new WzImage(file.Name));

            if (file.Name == ImageUtils.QuestInfo)
            {
                AllNodes = file.WzProperties.Select(x => x.Name).ToList();
            }

            ApplyQuestImage(FinalData[file.Name]!, NewData[file.Name]!);
            MainForm.PendingListWin.ResetView();
        }

        void ApplyQuestImage(ImageContext context, WzImage newData)
        {
            var sourceImg = SourceFile.WzDirectory.GetImageByName(context.Image.Name);
            foreach (var item in AllNodes)
            {
                var sourceItem = sourceImg[item]?.DeepClone();
                var newItem = newData[item]?.DeepClone();

                if (sourceItem == null)
                {
                    if (newItem != null)
                    {
                        context.TagNewItem(newItem);
                    }
                }
                else
                {
                    if (newItem != null)
                    {
                        if (!context.IsIgnorePropertyOverwrite())
                        {
                            context.ApplyQuestItemProperty(sourceItem, sourceItem, newItem);
                        }
                    }
                    context.Image.AddProperty(sourceItem);
                }

            }
        }

        public bool CheckComplete()
        {
            var sayData = FinalData.GetValueOrDefault(ImageUtils.Say)?.Image.WzProperties.Select(x => x.Name) ?? [];
            var actData = FinalData.GetValueOrDefault(ImageUtils.Act)?.Image.WzProperties.Select(x => x.Name) ?? [];
            var checkData = FinalData.GetValueOrDefault(ImageUtils.Check)?.Image.WzProperties.Select(x => x.Name) ?? [];

            var sayMiss = AllNodes.Except(sayData);
            var actMiss = AllNodes.Except(actData);
            var checkMiss = AllNodes.Except(checkData);

            bool isCompleted = true;
            if (sayMiss.Count() != 0)
            {
                Log.Logger.Warning("{Image} 缺少 {Items}", ImageUtils.Say, string.Join(',', sayMiss));
                isCompleted = false;
            }

            if (actMiss.Count() != 0)
            {
                Log.Logger.Warning("{Image} 缺少 {Items}", ImageUtils.Act, string.Join(',', actMiss));
                isCompleted = false;
            }

            if (checkMiss.Count() != 0)
            {
                Log.Logger.Warning("{Image} 缺少 {Items}", ImageUtils.Check, string.Join(',', checkMiss));
                isCompleted = false;
            }

            return isCompleted;
        }

        public void Dispose()
        {
            foreach (var item in NewData)
            {
                item.Value?.Dispose();
            }
            foreach (var item in FinalData)
            {
                item.Value?.Dispose();
            }
            AllNodes.Clear();
            SourceFile.Dispose();

            Instance = null;
            MainForm.Clear();
        }
    }
}
