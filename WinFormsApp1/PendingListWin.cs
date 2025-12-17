using WeifenLuo.WinFormsUI.Docking;

namespace WinFormsApp1
{
    /// <summary>
    /// 待处理的项
    /// </summary>
    internal class PendingListWin : DockContent
    {
        Form1 _mainForm;
        ListBox _listBox;
        public PendingListWin(Form1 mainForm)
        {
            _mainForm = mainForm;
            Text = "待手动处理";

            _listBox = new ListBox()
            {
                Dock = DockStyle.Fill,
            };
            _listBox.SelectedValueChanged += (s, e) =>
            {
                if (WorkContext.Instance == null)
                {
                    return;
                }

                if (_listBox.SelectedItems.Count == 0)
                {
                    return;
                }

                var item = _listBox.SelectedItem?.ToString();
                if (item == null)
                {
                    return;
                }

                if (_mainForm.DockPanelCtrl.ActiveDocument is WorkSpaceWin doc)
                {
                    doc.CurrentContext.CurrentNode = _dataSource[item];
                    doc.ShowCurrentNode();
                }
            };
            Controls.Add(_listBox);

        }

        Dictionary<string, string> _dataSource = new();
        public void ResetView()
        {
            _dataSource.Clear();
            _listBox.Items.Clear();

            if (WorkContext.Instance != null)
            {
                Task.Run(() =>
                {
                    if (WorkContext.Instance != null)
                    {
                        BeginInvoke(() =>
                        {
                            if (_mainForm.DockPanelCtrl.ActiveDocument is WorkSpaceWin doc)
                            {
                                _listBox.BeginUpdate();
                                _listBox.Items.Clear();
                                _dataSource.Clear();

                                var current = doc.CurrentContext.GetValidPendingItems();
                                foreach (var item in current)
                                {
                                    var str = item.Key;
                                    if (item.Value.Type == PendingType.NewNode)
                                    {
                                        str += " [+]";
                                    }
                                    if (!item.Value.Processed)
                                    {
                                        str += "（待处理）";
                                    }
                                    _dataSource[str] = item.Key;
                                    _listBox.Items.Add(str);
                                }
                                _listBox.EndUpdate();
                            }

                            HandleNodeChange();
                        });
                    }
                });
            }


        }

        public void HandleNodeChange()
        {
            if (_mainForm.DockPanelCtrl.ActiveDocument is WorkSpaceWin doc)
            {
                int index = doc.CurrentContext.CurrentIndex;
                if (index >= 0 && _listBox.Items.Count > 0)
                {
                    _listBox.SelectedIndex = index; // 选中
                    _listBox.TopIndex = index;      // 自动滚动到该项
                }
            }
        }
    }
}
