using System;
using System.Drawing;
using System.Windows.Forms;
using MySqlConnector;

namespace ComputerStore
{
    public sealed class CategoriesForm : Form
    {
        private readonly DataGridView grid = new DataGridView();
        private readonly TextBox name = new TextBox();
        private int selectedId;
        public CategoriesForm()
        {
            Text = "Категории"; ClientSize = new Size(740, 390); StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedSingle; MaximizeBox = false;
            grid.Location = new Point(15, 15); grid.Size = new Size(710, 190); grid.ReadOnly = true; grid.AllowUserToAddRows = grid.AllowUserToDeleteRows = false; grid.MultiSelect = false; grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; grid.CellClick += SelectRow; Controls.Add(grid);
            Controls.Add(new Label { Text = "Название", AutoSize = true, Location = new Point(15, 225) }); name.Location = new Point(125, 220); name.Size = new Size(600, 25); name.MaxLength = 100; Controls.Add(name);
            AddButton("Добавить", 15, Add); AddButton("Сохранить", 195, Save); AddButton("Удалить", 375, Delete); AddButton("Обновить", 555, (s,e) => { LoadData(); name.Clear(); });
            Load += (s,e) => LoadData();
        }
        private void AddButton(string text, int x, EventHandler click) { var b = new Button { Text=text, Location=new Point(x,310), Size=new Size(x == 555 ? 170 : 165,40) }; b.Click += click; Controls.Add(b); }
        private void LoadData() { try { grid.DataSource = Db.Query("SELECT id AS ID, name AS Название FROM categories ORDER BY name"); grid.ClearSelection(); selectedId=0; } catch(Exception ex) { Db.ShowError(ex); } }
        private void SelectRow(object sender, DataGridViewCellEventArgs e) { if(e.RowIndex < 0) return; var row=grid.Rows[e.RowIndex]; selectedId=Convert.ToInt32(row.Cells["ID"].Value); name.Text=Convert.ToString(row.Cells["Название"].Value); }
        private bool Valid() { if(name.Text.Trim().Length != 0) return true; MessageBox.Show("Введите название категории."); return false; }
        private void Add(object sender, EventArgs e) { if(!Valid()) return; try { Db.Execute("INSERT INTO categories(name) VALUES(@name)", new MySqlParameter("@name",name.Text.Trim())); LoadData(); name.Clear(); } catch(Exception ex) { Db.ShowError(ex); } }
        private void Save(object sender, EventArgs e) { if(selectedId == 0) { MessageBox.Show("Выберите категорию в таблице."); return; } if(!Valid()) return; try { Db.Execute("UPDATE categories SET name=@name WHERE id=@id",new MySqlParameter("@name",name.Text.Trim()),new MySqlParameter("@id",selectedId)); LoadData(); name.Clear(); } catch(Exception ex) { Db.ShowError(ex); } }
        private void Delete(object sender, EventArgs e) { if(selectedId == 0) { MessageBox.Show("Выберите категорию в таблице."); return; } if(MessageBox.Show("Удалить категорию?","Подтверждение",MessageBoxButtons.YesNo) != DialogResult.Yes) return; try { Db.Execute("DELETE FROM categories WHERE id=@id",new MySqlParameter("@id",selectedId)); LoadData(); name.Clear(); } catch(Exception ex) { Db.ShowError(ex); } }
    }
}
