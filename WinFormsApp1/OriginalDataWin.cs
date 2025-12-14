using MapleLib.WzLib;
using WeifenLuo.WinFormsUI.Docking;

namespace WinFormsApp1
{
    internal class OriginalDataWin : DockContent
    {
        TableLayoutPanel _bodyPanel;



        ContextMenuStrip menu = new ContextMenuStrip();
        Form1 _mainForm;
        public OriginalDataWin(Form1 main)
        {
            Text = "已加载文件";
            _mainForm = main;

            _bodyPanel = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                RowCount = 2
            };
            // 行 50% / 50%
            _bodyPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            _bodyPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            menu.Items.Add("导入");

            Controls.Add(_bodyPanel);
        }

        TreeView? _tree;
        public void DrawData()
        {
            if (WorkContext.Instance == null)
                return;

            if (_tree == null)
            {
                _tree = new() { Dock = DockStyle.Fill };
                _bodyPanel.Controls.Add(_tree, 0, 0);
            }
            _tree.Nodes.Clear();
            _tree.Nodes.Add(
                new TreeNode(WorkContext.Instance.SourceFile.Name,
                 WorkContext.Instance.SourceFile.WzDirectory.WzImages.Select(x => new TreeNode(x.Name)).ToArray()));
            _tree.ExpandAll();

            DrawNewDataByWz();
        }



        TreeView? _newTree;

        public void DrawNewDataByWz()
        {
            if (WorkContext.Instance == null)
                return;

            if (_newTree == null)
            {
                _newTree = new() { Dock = DockStyle.Fill };
                _newTree.NodeMouseClick += OnNewTree_NodeMouseClick;
                _bodyPanel.Controls.Add(_newTree, 0, 1);
            }
            _newTree.Nodes.Clear();

            var node = new TreeNode("用于更新的文件",
                 WorkContext.Instance.SourceFile.WzDirectory.WzImages.Select(x => new TreeNode(WorkContext.Instance.NewData.GetValueOrDefault(x.Name) == null ? x.Name + "（未导入）" : x.Name)).ToArray());
            _newTree.Nodes.Add(node);
            _newTree.ExpandAll();

            foreach (var item in WorkContext.Instance.NewData)
            {
                if (item.Value != null)
                {
                    _mainForm.ShowDocument(item.Value);
                }
            }

            _mainForm.PendingListWin.ReloadData();
        }

        public void OnNewTree_NodeMouseClick(object? sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.Node == null)
                {
                    return;
                }

                if (WorkContext.Instance == null)
                {
                    return;
                }

                // 让该节点成为当前选中的节点
                _newTree!.SelectedNode = e.Node;

                menu.Items.Clear();

                if (e.Node.Level == 0)
                {
                    var itemAdd = new ToolStripMenuItem("导入Quest.wz");
                    menu.Items.Add(itemAdd);
                    itemAdd.Click += (s, o) =>
                    {
                        var file = SelectWzFile();
                        if (file != null)
                        {
                            WorkContext.Instance.SetNewData(file);
                            DrawNewDataByWz();
                        }
                    };
                }
                else
                {

                    string selecteImage;
                    if (e.Node.Text.Contains("未导入"))
                    {
                        selecteImage = e.Node.Text[..^5];

                    }
                    else
                    {
                        selecteImage = e.Node.Text;
                    }

                    var itemAdd = new ToolStripMenuItem("导入" + selecteImage);
                    menu.Items.Add(itemAdd);

                    itemAdd.Click += (s, o) =>
                    {
                        if (selecteImage != "QuestInfo.img" && !WorkContext.Instance.NewData.ContainsKey("QuestInfo.img"))
                        {

                            var r = MessageBox.Show("必须先导入QuestInfo.img，或者你不想修改QuestInfo.img？（点击确定继续）", "选择", MessageBoxButtons.OKCancel);
                            if (r == DialogResult.OK)
                            {
                                var file = SelectWzImage(selecteImage);
                                if (file != null)
                                {
                                    WorkContext.Instance.SetNewData(file);
                                    DrawNewDataByWz();
                                }
                            }
                            return;
                        }
                        else
                        {
                            var file = SelectWzImage(selecteImage);
                            if (file != null)
                            {
                                WorkContext.Instance.SetNewData(file);
                                DrawNewDataByWz();
                            }
                        }
                    };

                }



                // 在鼠标位置弹出菜单
                menu.Show(_newTree, e.Location);
            }
        }

        public static WzFile? SelectWzFile()
        {
            WzFile? _workingWz = null;
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
                        var _workingWz = new WzFile(selectFileDialog.FileName, o.GameVersion, o.Version);

                        _workingWz.ParseWzFile();
                        if (!_workingWz.WzDirectory.Name.Equals("Quest.wz", StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show("仅支持Quest.wz");
                        }

                        //foreach (var img in _workingWz.WzDirectory.WzImages)
                        //{
                        //    var doc = new WorkSpaceWin(img.Name);
                        //    doc.Show(dockPanel, DockState.Document);
                        //}

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("仅支持Quest.wz");
                    }
                };
                versionWin.ShowDialog();
            }
            return _workingWz;
        }

        public static WzImage? SelectWzImage(string imgName)
        {
            WzImage? newImg = null;
            var selectFileDialog = new OpenFileDialog()
            {
                Filter = "img文件(*.img)|*.img"
            };
            var r = selectFileDialog.ShowDialog();
            if (r == DialogResult.OK)
            {
                var versionWin = new WzVersionInputWin(false);
                versionWin.OnSubmit += (s, o) =>
                {
                    try
                    {
                        var newImgStream = new FileStream(selectFileDialog.FileName, FileMode.Open, FileAccess.Read);
                        newImg = new WzImage(imgName, newImgStream, o.Version);
                        //newImg.ParseImage();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("读取Image失败");
                    }
                };
                versionWin.ShowDialog();
            }
            return newImg;
        }
    }
}
