using MapleLib.WzLib;
using WeifenLuo.WinFormsUI.Docking;

namespace WinFormsApp1
{
    internal class WorkSpaceWin : DockContent
    {
        TableLayoutPanel table = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 2,
            
        };


        #region 原始视图
        DataGridView gridA = new()
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false
        };
        Panel originalPanel = new Panel { BackColor = Color.LightBlue, Dock = DockStyle.Fill };
        #endregion

        #region 更新文件视图
        DataGridView gridB = new()
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false
        };
        Panel newPanel = new Panel { BackColor = Color.LightGreen, Dock = DockStyle.Fill };
        #endregion

        #region 顶部控制栏
        FlowLayoutPanel toolPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Height = 36,
        };
        Label infoLabel = new Label() { AutoSize = true };
        Button btnNext = new Button() { Text = "下一条", AutoSize = true };
        Button btnPrevious = new Button() { Text = "上一条", AutoSize = true };

        Button btnNextConflict = new Button() { Text = "下一个需要处理", AutoSize = true };
        Button btnPreviousConflict = new Button() { Text = "上一个需要处理", AutoSize = true };
        #endregion

        #region 最终视图
        Panel finalPanel = new Panel { BackColor = Color.LightSalmon, Dock = DockStyle.Fill };
        DataGridView gridC = new()
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows = false,
        };
        FlowLayoutPanel finalToolPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Height = 36
        };
        Button btnCompleted = new Button() { Text = "解决并移动到下一条", AutoSize = true, Anchor = AnchorStyles.Top };
        Label finalWording = new Label() { AutoSize = true, Anchor = AnchorStyles.Top };
        Button btnUseB = new Button() { Text = "使用新增项（新增）并解决", AutoSize = true };
        Button btnRemoveB = new Button() { Text = "移除新增项（放弃）并解决", AutoSize = true };
        #endregion


        WzImage _imgA;
        WzImage _imgB;
        ImageContext _imgC;
        public ImageContext CurrentContext => _imgC;
        public WorkSpaceWin(string imgName)
        {
            _imgB = WorkContext.Instance!.NewData.GetValueOrDefault(imgName)!;
            _imgA = WorkContext.Instance!.SourceFile.WzDirectory.GetImageByName(imgName);
            _imgC = WorkContext.Instance.FinalData.GetValueOrDefault(imgName)!;

            this.Text = imgName;

            // 行 50% / 50%
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 5));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            // 列 50% / 50%
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));


            btnNext.Click += (o, s) =>
            {
                _imgC.CurrentIndex++;
                ShowCurrentNode();
            };
            btnPrevious.Click += (o, s) =>
            {
                _imgC.CurrentIndex--;
                ShowCurrentNode();
            };
            btnNextConflict.Click += HandleClickNextConflict;
            btnPreviousConflict.Click += HandleClickPreConflict;
            toolPanel.Controls.Add(infoLabel);
            toolPanel.Controls.Add(btnPrevious);
            toolPanel.Controls.Add(btnNext);
            toolPanel.Controls.Add(btnPreviousConflict);
            toolPanel.Controls.Add(btnNextConflict);


            finalToolPanel.Controls.Add(btnCompleted);
            finalToolPanel.Controls.Add(finalWording);
            finalToolPanel.Controls.Add(btnUseB);
            finalToolPanel.Controls.Add(btnRemoveB);
            btnUseB.Click += HandleUseB;
            btnRemoveB.Click += HandleRemoveB;
            btnCompleted.Click += HandleComplete;

            // 添加到布局
            table.Controls.Add(toolPanel, 0, 0);
            table.SetColumnSpan(toolPanel, 2);

            table.Controls.Add(originalPanel, 0, 1);
            table.Controls.Add(newPanel, 1, 1);

            // C 占据整行
            table.Controls.Add(finalPanel, 0, 2);
            table.SetColumnSpan(finalPanel, 2);

            gridA.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Type",
                Name = "Type",
                FillWeight = 20,
            });
            gridA.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Path",
                Name = "Name",
                FillWeight = 30,
            });
            gridA.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Value",
                Name = "Value",
                FillWeight = 50,
            });

            gridB.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Type",
                Name = "Type",
                FillWeight = 20,
            });
            gridB.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Path",
                Name = "Name",
                FillWeight = 30,
            });
            gridB.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Value",
                Name = "Value",
                FillWeight = 50,
            });

            gridC.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Type",
                Name = "Type",
                FillWeight = 20,
            });
            gridC.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Path",
                Name = "Name",
                FillWeight = 30,
            });
            gridC.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Value",
                Name = "Value",
                FillWeight = 50,

            });
            gridC.Columns[0]!.ReadOnly = true;
            gridC.Columns[1]!.ReadOnly = true;
            gridC.CellValueChanged += HandleFinalValueChanged;


            originalPanel.Controls.Add(gridA);

            newPanel.Controls.Add(gridB);

            finalPanel.Controls.Add(gridC);
            finalPanel.Controls.Add(finalToolPanel);

            this.Controls.Add(table);

        }

        private void HandleRemoveB(object? sender, EventArgs e)
        {
            var nodeName = WorkContext.Instance?.CurrentNode ;
            if (nodeName == null)
            {
                return;
            }
            _imgC.RemoveNewItem();
            ShowCurrentNode();
        }

        private void HandleUseB(object? sender, EventArgs e)
        {
            var nodeName = WorkContext.Instance?.CurrentNode;
            if (nodeName == null)
            {
                return;
            }
            _imgC.InserNewItem();
            ShowCurrentNode();
        }


        private void HandleComplete(object? sender, EventArgs e)
        {
            _imgC.ResolvePendingItem();

            HandleClickNextConflict(sender, e);
        }

        private void HandleFinalValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            var path = gridC.Rows[e.RowIndex].Cells[1].Value?.ToString();
            if (path == null)
            {
                return;
            }

            var value = gridC.Rows[e.RowIndex].Cells[2].Value?.ToString() ?? "";
            _imgC.SetPropertyValue(path, value);

            ShowCurrentNode();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ResetView();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!_allowClose && this.DockState == DockState.Document)
            {
                e.Cancel = true;
                return;
            }

            base.OnFormClosing(e);
        }

        private bool _allowClose = false;   
        public void ActualClose()
        {
            _allowClose = true;
            this.Close();
        }


        public void ReloadDataSource(string imgName)
        {
            _imgB = WorkContext.Instance!.NewData.GetValueOrDefault(imgName)!;
            _imgA = WorkContext.Instance!.SourceFile.WzDirectory.GetImageByName(imgName);
            _imgC = WorkContext.Instance.FinalData.GetValueOrDefault(imgName)!;

            ResetView();
        }

        public void ResetView()
        {
            _imgC.CurrentIndex = 0;
            ShowCurrentNode();
        }

        void HandleClickNextConflict(object? sender, EventArgs e)
        {
            if (WorkContext.Instance == null)
                return;

            _imgC.CurrentIndex++;
            ShowNode();
        }

        void HandleClickPreConflict(object? sender, EventArgs e)
        {
            _imgC.CurrentIndex--;
            ShowNode();

        }

        void ShowNode()
        {
            if (WorkContext.Instance == null)
                return;

            btnUseB.Visible = false;
            btnRemoveB.Visible = false;
            finalPanel.BackColor = Color.White;
            finalWording.Text = "";
            var allPendingItems = _imgC.GetValidPendingItems();
            if (allPendingItems.TryGetValue(WorkContext.Instance.CurrentNode, out var pendingNode))
            {
                if (pendingNode.Type == PendingType.NewNode)
                {
                    finalWording.Text = "当前节点是新增的节点。";

                    var hasData = _imgC.Image.WzProperties.Any(x => x.Name == pendingNode.Node.Name);
                    btnUseB.Visible = !hasData;
                    btnRemoveB.Visible = hasData;
                }

                if (!pendingNode.Processed)
                {
                    finalPanel.BackColor = Color.LightPink;
                }
            }

            finalWording.Text += "点击解决使更改生效。";

            infoLabel.Text = $"共有{WorkContext.Instance.GetEffectiveNodes().Count}/{WorkContext.Instance!.AllNodes.Count}条，有{allPendingItems.Count(x => !x.Value.Processed)}/{allPendingItems.Count}条需要手动处理。当前QuestId={WorkContext.Instance.CurrentNode}";

            var allA = ImageUtils.FlatSelectNode(_imgA.GetFromPath(WorkContext.Instance.CurrentNode));
            var allB = ImageUtils.FlatSelectNode(_imgB.GetFromPath(WorkContext.Instance.CurrentNode));
            var allC = ImageUtils.FlatSelectNode(_imgC.Image.GetFromPath(WorkContext.Instance.CurrentNode));


            gridA.Rows.Clear();
            gridB.Rows.Clear();
            gridC.Rows.Clear();

            var allProps = allA.Select(x => x.Name).Union(allB.Select(x => x.Name)).ToList();
            List<int> idxs = [];
            for (int i = 0; i < allProps.Count; i++)
            {
                var prop = allProps[i];

                var propA = allA.FirstOrDefault(x => x.Name == prop);
                var propB = allB.FirstOrDefault(x => x.Name == prop);
                var propC = allC.FirstOrDefault(x => x.Name == prop);

                var rowA = new DataGridViewRow();
                if (propA != null)
                {
                    rowA.Cells.Add(new DataGridViewTextBoxCell() { Value = propA.Type });
                    rowA.Cells.Add(new DataGridViewTextBoxCell() { Value = propA.Name });
                    rowA.Cells.Add(new DataGridViewTextBoxCell() { Value = propA.Value });
                }
                gridA.Rows.Add(rowA);

                var rowB = new DataGridViewRow();
                if (propB != null)
                {
                    rowB.Cells.Add(new DataGridViewTextBoxCell() { Value = propB.Type });
                    rowB.Cells.Add(new DataGridViewTextBoxCell() { Value = propB.Name });
                    rowB.Cells.Add(new DataGridViewTextBoxCell() { Value = propB.Value });
                }
                gridB.Rows.Add(rowB);

                if (!ImageUtils.ZhCompare(propA?.Value, propB?.Value) || propA?.Name != propB?.Name || propA?.Type != propB?.Type)
                {
                    rowA.DefaultCellStyle.BackColor = Color.LightGray;
                    rowB.DefaultCellStyle.BackColor = Color.LightGray;
                }

                var rowC = new DataGridViewRow();
                if (propC != null)
                {
                    rowC.Cells.Add(new DataGridViewTextBoxCell() { Value = propC.Type });
                    rowC.Cells.Add(new DataGridViewTextBoxCell() { Value = propC.Name });
                    rowC.Cells.Add(new DataGridViewTextBoxCell() { Value = propC.Value });

                    if (propC.Type != WzPropertyType.String.ToString())
                    {
                        rowC.Cells[2].ReadOnly = true;
                    }

                    if (pendingNode != null && !pendingNode.Processed && (pendingNode.DiffSubProps.Any(x => x.FullPath == prop) || pendingNode.Type == PendingType.NewNode))
                    {
                        rowC.DefaultCellStyle.BackColor = Color.LightPink;
                    }
                }
                gridC.Rows.Add(rowC);
            }
        }


        public void ShowCurrentNode()
        {
            if (WorkContext.Instance == null || WorkContext.Instance.CurrentIndex < 0)
            {
                MessageBox.Show("未开始工作/不存在的节点");
                return;
            }

            ShowNode();
        }
    }
}
