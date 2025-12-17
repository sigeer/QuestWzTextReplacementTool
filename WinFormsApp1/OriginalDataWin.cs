using MapleLib.WzLib;
using WeifenLuo.WinFormsUI.Docking;

namespace WinFormsApp1
{
    internal class OriginalDataWin : DockContent
    {
        TableLayoutPanel _layoutPanel;


        ContextMenuStrip menu = new ContextMenuStrip();
        Form1 _mainForm;
        public OriginalDataWin(Form1 main)
        {
            Text = "已加载文件";
            _mainForm = main;

            _layoutPanel = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                RowCount = 2
            };
            // 行 50% / 50%
            _layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            _layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            menu.Items.Add("导入");

            Controls.Add(_layoutPanel);
        }

        TreeView? _tree;
        public void DrawData()
        {
            _tree?.Nodes?.Clear();

            if (WorkContext.Instance != null)
            {
                if (_tree == null)
                {
                    _tree = new() { Dock = DockStyle.Fill };
                    _layoutPanel.Controls.Add(_tree, 0, 0);
                }

                _tree.Nodes.Add(
                    new TreeNode(WorkContext.Instance.SourceFile.Name,
                     WorkContext.Instance.SourceFile.WzDirectory.WzImages.Select(x => new TreeNode(x.Name)).ToArray()));
                _tree.ExpandAll();
            }

            DrawNewDataByWz();
        }



        TreeView? _newTree;

        public void DrawNewDataByWz()
        {
            _newTree?.Nodes?.Clear();
            if (WorkContext.Instance != null)
            {
                if (_newTree == null)
                {
                    _newTree = new() { Dock = DockStyle.Fill };
                    _newTree.NodeMouseClick += OnNewTree_NodeMouseClick;
                    _layoutPanel.Controls.Add(_newTree, 0, 1);
                }

                var node = new TreeNode("用于更新的文件",
                     WorkContext.Instance.SourceFile.WzDirectory.WzImages.Select(x => new TreeNode(WorkContext.Instance.NewData.GetValueOrDefault(x.Name) == null ? x.Name + "（未导入）" : x.Name)).ToArray());
                _newTree.Nodes.Add(node);
                _newTree.ExpandAll();

                foreach (var item in WorkContext.Instance.NewData)
                {
                    if (item.Value != null)
                    {
                        _mainForm.ShowDocument(item.Key);
                    }
                }

                _mainForm.PendingListWin.Activate();
            }
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
                        SelectWzFile(file =>
                        {
                            WorkContext.Instance.SetNewData(file);
                            _mainForm.IgnoreCheck();
                            DrawNewDataByWz();
                            _mainForm.RecoveryCheck();
                        });
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

                    if (!ImageUtils.EffectImage(selecteImage))
                    {
                        MessageBox.Show("仅支持修改" + string.Join(", ", ImageUtils.EffectImages));
                        return;
                    }

                    var itemAdd = new ToolStripMenuItem("导入" + selecteImage);
                    menu.Items.Add(itemAdd);

                    itemAdd.Click += (s, o) =>
                    {
                        if (selecteImage != ImageUtils.QuestInfo && !WorkContext.Instance.NewData.ContainsKey(ImageUtils.QuestInfo))
                        {

                            var r = MessageBox.Show("必须先导入QuestInfo.img，或者你不想修改QuestInfo.img？（点击确定继续）", "选择", MessageBoxButtons.OKCancel);
                            if (r == DialogResult.OK)
                            {
                                SelectWzImage(selecteImage, file =>
                                {
                                    WorkContext.Instance.SetNewData(file);
                                    DrawNewDataByWz();
                                });

                            }
                            return;
                        }
                        else
                        {
                            SelectWzImage(selecteImage, file =>
                            {
                                WorkContext.Instance.SetNewData(file);
                                DrawNewDataByWz();
                            });
                        }
                    };

                }

                // 在鼠标位置弹出菜单
                menu.Show(_newTree, e.Location);
            }
        }

        public static void SelectWzFile(Action<WzFile> action)
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
                        var inputWz = new WzFile(selectFileDialog.FileName, o.GameVersion, o.Version);

                        inputWz.ParseWzFile();
                        if (!inputWz.WzDirectory.Name.Equals("Quest.wz", StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show("仅支持Quest.wz");
                        }
                        action(inputWz);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("仅支持Quest.wz, " + ex.ToString());
                    }
                };
                versionWin.ShowDialog();
            }
        }

        public static void SelectWzImage(string imgName, Action<WzImage> action)
        {
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
                        action(new WzImage(imgName, newImgStream, o.Version));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("读取Image失败，" + ex.ToString());
                    }
                };
                versionWin.ShowDialog();
            }
        }
    }
}
