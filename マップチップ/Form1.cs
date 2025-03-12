using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace マップチップ
{

    public partial class Form1 : Form
    {
        System.Windows.Forms.ToolTip toolTip = new System.Windows.Forms.ToolTip();

        public Form1()
        {
            InitializeComponent();


            dataGridView1.Rows.Add("A00123456789abcdefg", "商品A");
            dataGridView1.Rows.Add("B002", "商品B");

        }

        

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            int targetColumnIndex = 0; // ずらしたい列のインデックス
            int offsetX = 50; // 右にずらすピクセル数

            if (e.RowIndex >= 0 && e.ColumnIndex == targetColumnIndex)
            {
                // 背景を描画
                e.PaintBackground(e.ClipBounds, true);

                // 文字列の描画領域を作成
                Rectangle textRect = new Rectangle(
                    e.CellBounds.X + offsetX, // 右にずらす
                    e.CellBounds.Y,
                    e.CellBounds.Width - offsetX,
                    e.CellBounds.Height
                );

                // 文字列を描画
                TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis;
                TextRenderer.DrawText(e.Graphics, e.FormattedValue?.ToString(), e.CellStyle.Font, textRect, e.CellStyle.ForeColor, flags);

                e.Handled = true; // 既定の描画を抑制
            }
            else
            {
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All);
            }
        }

        private void dataGridView1_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {

        }


        private void dataGridView1_MouseMove(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hit = dataGridView1.HitTest(e.X, e.Y);

            if (hit.Type == DataGridViewHitTestType.Cell && hit.RowIndex >= 0 && hit.ColumnIndex == 0)
            {
                DataGridViewCell cell = dataGridView1.Rows[hit.RowIndex].Cells[hit.ColumnIndex];
                string cellText = cell.Value?.ToString();

                if (!string.IsNullOrEmpty(cellText) && IsTextClipped(hit.RowIndex, hit.ColumnIndex, cellText))
                {
                    toolTip.Show(cellText, dataGridView1, e.X + 10, e.Y + 20, 2000); // 2秒表示
                }
                else
                {
                    toolTip.Hide(dataGridView1);
                }
            }
            else
            {
                toolTip.Hide(dataGridView1);
            }
        }

        private bool IsTextClipped(int rowIndex, int columnIndex, string text)
        {
            using (Graphics g = dataGridView1.CreateGraphics())
            {
                DataGridViewCell cell = dataGridView1.Rows[rowIndex].Cells[columnIndex];
                Font font = cell.InheritedStyle.Font;
                int cellWidth = cell.Size.Width - 50; // 右側の 16 ピクセルを考慮

                // テキストの描画サイズを測定
                Size textSize = TextRenderer.MeasureText(g, text, font);

                // テキストがセルの幅より大きければ省略される
                return textSize.Width > cellWidth;
            }
        }
    }
}
