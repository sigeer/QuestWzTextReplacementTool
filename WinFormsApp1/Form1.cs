using MapleLib.WzLib;
using Serilog;
using System;
using System.ComponentModel;
using System.Security.Policy;
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
            DockPanelCtrl.Theme = new VS2015LightTheme(); // VS 风格主题
            DockPanelCtrl.ActiveDocumentChanged += HandleActiveDocumentChanged;
            this.toolStripContainer1.ContentPanel.Controls.Add(DockPanelCtrl);

            tool = new OriginalDataWin(this) { CloseButton = false, CloseButtonVisible = false };
            PendingListWin = new(this) { CloseButton = false, CloseButtonVisible = false };
        }

        private void HandleActiveDocumentChanged(object? sender, EventArgs e)
        {
            var doc = DockPanelCtrl.ActiveDocument as WorkSpaceWin;
            PendingListWin.ReloadData();
        }

        OriginalDataWin tool;
        OutputWin output = new();
        private void Form1_Load(object sender, EventArgs e)
        {
            tool.Show(DockPanelCtrl, DockState.DockLeft);
            PendingListWin.Show(DockPanelCtrl, DockState.DockLeft);
            tool.Activate();

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

                        tool.DrawData();
                        ReloadDocuments();
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
        public void ShowDocument(WzImage newlyImage)
        {
            if (!_allDocuments.TryGetValue(newlyImage.Name, out var doc))
            {
                doc = new WorkSpaceWin(newlyImage) { CloseButton = false, CloseButtonVisible = false };

                _allDocuments[newlyImage.Name] = doc;
                doc.Show(DockPanelCtrl, DockState.Document);
            }
            else
            {
                doc.ResetDataSource();
            }

        }

        public void ReloadDocuments()
        {
            foreach (var item in _allDocuments)
            {
                item.Value.ResetDataSource();
            }
        }

        private void Menu_Run_Click(object sender, EventArgs e)
        {
            //if (WorkContext.Instance == null)
            //{
            //    MessageBox.Show("请先选择 开始->选择Quest.wz");
            //    return;
            //}
            //foreach (var item in WorkContext.Instance.NewData)
            //{
            //    if (item.Value == null)
            //    {
            //        continue;
            //    }
            //    var context = new ImageContext(
            //        WorkContext.Instance.SourceFile.WzDirectory.GetImageByName(item.Key).DeepClone());
            //    WorkContext.Instance.FinalData[item.Key] = context;
            //    ApplyQuestImage(context, item.Value);
            //}

            //ReloadDocuments();
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (WorkContext.Instance == null)
            {
                MessageBox.Show("尚未开始");
                return;
            }

            Log.Logger.Information("开始保存wz");
            if (!WorkContext.Instance.CheckComplete())
            {
                output.Show(DockPanelCtrl, DockState.DockBottomAutoHide);
                return;
            }

            var selectFileDialog = new SaveFileDialog()
            {
                Filter = "wz文件(*.wz)|*.wz",
                OverwritePrompt = true,
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

                        foreach (var item in WorkContext.Instance.SourceFile.WzDirectory.WzImages)
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
