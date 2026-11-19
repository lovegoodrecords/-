using System;
using System.Drawing;
using System.Windows.Forms;

namespace ComputerStore
{
    public sealed class MainForm : Form
    {
        public MainForm()
        {
            Text = "Магазин компьютерной техники"; ClientSize = new Size(700, 280);
            StartPosition = FormStartPosition.CenterScreen; FormBorderStyle = FormBorderStyle.FixedSingle; MaximizeBox = false;
            var title = new Label { Text = "Магазин компьютерной техники", AutoSize = true, Location = new Point(25, 25), Font = new Font(Font, FontStyle.Bold) };
            Controls.Add(title);
            AddButton("Категории", new Point(25, 85), (s, e) => { using (var f = new CategoriesForm()) f.ShowDialog(this); });
            AddButton("Товары", new Point(240, 85), (s, e) => { using (var f = new ProductsForm()) f.ShowDialog(this); });
            AddButton("Продажи", new Point(455, 85), (s, e) => { using (var f = new SalesForm()) f.ShowDialog(this); }, 220);
            AddButton("Выход", new Point(475, 190), (s, e) => Close());
        }
        private void AddButton(string text, Point location, EventHandler click, int width = 200)
        { var b = new Button { Text = text, Location = location, Size = new Size(width, 45) }; b.Click += click; Controls.Add(b); }
    }
}
