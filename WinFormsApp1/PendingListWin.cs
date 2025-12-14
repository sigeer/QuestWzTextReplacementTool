using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using WeifenLuo.WinFormsUI.Docking;

namespace WinFormsApp1
{
    /// <summary>
    /// 待处理的项
    /// </summary>
    internal class PendingListWin : DockContent
    {
        Form1 _mainForm;
        ListBox _listView;
        public PendingListWin(Form1 mainForm)
        {
            _mainForm = mainForm;
            Text = "待手动处理";

            _listView = new ListBox()
            {
                Dock = DockStyle.Fill,
            };
            _listView.SelectedValueChanged += (s, e) =>
            {
                if (WorkContext.Instance == null)
                {
                    return;
                }

                if (_listView.SelectedItems.Count == 0)
                {
                    return;
                }

                var item = _listView.SelectedItem?.ToString();
                if (item == null)
                {
                    return;
                }

                if (_mainForm.DockPanelCtrl.ActiveDocument is WorkSpaceWin doc)
                {
                    WorkContext.Instance.CurrentNode = item;
                    doc.ShowCurrentNode();
                }
            };
            Controls.Add(_listView);

        }

        public void ReloadData()
        {
            if (WorkContext.Instance == null)
            {
                return;
            }

            Task.Run(() =>
            {
                if (WorkContext.Instance == null)
                {
                    return;
                }


                BeginInvoke(() =>
                {
                    if (_mainForm.DockPanelCtrl.ActiveDocument is WorkSpaceWin doc)
                    {
                        _listView.BeginUpdate();
                        _listView.Items.Clear();

                        var current = WorkContext.Instance!.FinalData.GetValueOrDefault(doc.Text)?.GetAllPendingItems() ?? [];
                        foreach (var item in current)
                        {
                            var str = item.Key;
                            if (!item.Value.Processed)
                            {
                                str += "（待处理）";
                            }
                            _listView.Items.Add(str);
                        }
                        _listView.EndUpdate();
                    }
                });
            });
        }

        public void HandleNodeChange()
        {
            if (_mainForm.DockPanelCtrl.ActiveDocument is WorkSpaceWin doc)
            {
                int index = WorkContext.Instance?.FinalData?.GetValueOrDefault(doc.Text)?.GetPendingIndex(WorkContext.Instance.CurrentNode) ?? -1;
                if (index >= 0 && _listView.Items.Count > 0)
                {
                    _listView.SelectedIndex = index; // 选中
                    _listView.TopIndex = index;      // 自动滚动到该项
                }
            }
        }
    }
}
