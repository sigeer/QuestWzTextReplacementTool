using MapleLib.WzLib;
using Serilog;
using WeifenLuo.WinFormsUI.Docking;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        internal DockPanel DockPanelCtrl;

        internal PendingListWin PendingListWin;
        public Form1()
        {
            InitializeComponent();

            DockPanelCtrl = new DockPanel();
            DockPanelCtrl.Dock = DockStyle.Fill;
            DockPanelCtrl.Theme = new VS2015LightTheme();
            DockPanelCtrl.ActiveDocumentChanged += HandleActiveDocumentChanged;
            this.toolStripContainer1.ContentPanel.Controls.Add(DockPanelCtrl);

            DataSourceWin = new OriginalDataWin(this) { CloseButton = false, CloseButtonVisible = false };
            PendingListWin = new(this) { CloseButton = false, CloseButtonVisible = false };
        }

        private void HandleActiveDocumentChanged(object? sender, EventArgs e)
        {
            var doc = DockPanelCtrl.ActiveDocument as WorkSpaceWin;
            PendingListWin.ResetView();
        }

        OriginalDataWin DataSourceWin;
        OutputWin output = new();
        private void Form1_Load(object sender, EventArgs e)
        {
            DataSourceWin.Show(DockPanelCtrl, DockState.DockLeft);
            PendingListWin.Show(DockPanelCtrl, DockState.DockLeft);
            DataSourceWin.Activate();

            output.Show(DockPanelCtrl, DockState.DockBottomAutoHide);
        }

        WzFile? _workingWz;
        private void Menu_SelectWz_Click(object sender, EventArgs e)
        {
            var selectFileDialog = new OpenFileDialog()
            {
                Filter = "wz文件(*.wz)|*.wz"
            };
            var r = selectFileDialog.ShowDialog();
            if (r == DialogResult.OK)
            {
                var versionWin = new WzVersionInputWin();
                versionWin.OnSubmit += (s, o) =>
                {
                    try
                    {
                        _workingWz = new WzFile(selectFileDialog.FileName, o.GameVersion, o.Version);

                        _workingWz.ParseWzFile();
                        if (!_workingWz.WzDirectory.Name.Equals("Quest.wz", StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show("仅支持Quest.wz");
                        }

                        WorkContext.Instance = new WorkContext(this, _workingWz);

                        Clear();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"仅支持Quest.wz: {ex.Message}");
                    }
                };
                versionWin.ShowDialog();
            }
        }

        Dictionary<string, WorkSpaceWin> _allDocuments = [];
        public void ShowDocument(string imgName)
        {
            if (!_allDocuments.TryGetValue(imgName, out var doc))
            {
                doc = new WorkSpaceWin(imgName) { CloseButton = false, CloseButtonVisible = false };

                _allDocuments[imgName] = doc;
                doc.Show(DockPanelCtrl, DockState.Document);
            }
            else
            {
                doc.ReloadDataSource(imgName);
            }

        }

        public void Clear()
        {
            foreach (var item in _allDocuments)
            {
                item.Value.ActualClose();
            }
            _allDocuments.Clear();

            PendingListWin.ResetView();
            DataSourceWin.DrawData();
        }

        private void Menu_Strategy_Click(object sender, EventArgs e)
        {
            MessageBox.Show("""
                任务ID以QuestInfo.img包含的节点为主。
                自动替换仅处理QuestInfo.img，Say.img中的二级（及以下）节点的String节点的value。
                不增减二级（及以下）节点属性


                需要手动处理的情况：
                    1.新增一级节点（任务ID所在节点）
                    2.两个版本中对同一个属性，一个版本有值，另一个版本没有值


                其他复杂修改不支持。
                """, "更新策略");
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (WorkContext.Instance == null)
            {
                MessageBox.Show("尚未开始");
                return;
            }

            if (WorkContext.Instance.FinalData.Count == 0)
            {
                MessageBox.Show("没有修改");
                return;
            }

            Log.Logger.Information("开始保存wz");
            if (!WorkContext.Instance.CheckComplete())
            {
                var result = MessageBox.Show("一些img缺少与QuestInfo.img相应的节点（查看“输出”窗口）。点击确定继续导出。", "确认", MessageBoxButtons.OKCancel);
                output.Activate();

                if (result == DialogResult.OK)
                {
                    Save();
                }
                return;
            }
            Save();

        }

        void Save()
        {
            var selectFileDialog = new SaveFileDialog()
            {
                Filter = "wz文件(*.wz)|*.wz",
                OverwritePrompt = true,
                FileName = "Quest.wz"
            };
            var r = selectFileDialog.ShowDialog();
            if (r == DialogResult.OK)
            {
                var versionWin = new WzVersionInputWin();
                versionWin.OnSubmit += (s, o) =>
                {
                    try
                    {
                        using var outputFile = new WzFile(o.GameVersion, o.Version);

                        foreach (var item in WorkContext.Instance!.SourceFile.WzDirectory.WzImages)
                        {
                            if (WorkContext.Instance.FinalData.TryGetValue(item.Name, out var imgContext))
                            {
                                outputFile.WzDirectory.AddImage(imgContext.Image);
                            }
                            else
                            {
                                outputFile.WzDirectory.AddImage(item.DeepClone());
                            }
                        }
                        outputFile.SaveToDisk(selectFileDialog.FileName);
                        Log.Logger.Information("保存成功：{FilePath}", selectFileDialog.FileName);
                        WorkContext.Instance.Dispose();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"仅支持Quest.wz: {ex.Message}");
                    }
                };
                versionWin.ShowDialog();
            }
        }
    }
}
