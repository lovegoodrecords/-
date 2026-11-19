using System;
using System.Drawing;
using System.Windows.Forms;
using MySqlConnector;

namespace ComputerStore
{
    public sealed class ProductsForm : Form
    {
        private readonly DataGridView grid = new DataGridView(); private readonly TextBox name = new TextBox(); private readonly ComboBox category = new ComboBox(); private readonly NumericUpDown price = new NumericUpDown(); private readonly NumericUpDown stock = new NumericUpDown(); private int selectedId;
        public ProductsForm()
        {
            Text="Товары"; ClientSize=new Size(940,550); StartPosition=FormStartPosition.CenterParent; FormBorderStyle=FormBorderStyle.FixedSingle; MaximizeBox=false;
            grid.Location=new Point(15,15); grid.Size=new Size(910,210); grid.ReadOnly=true; grid.AllowUserToAddRows=grid.AllowUserToDeleteRows=false; grid.MultiSelect=false; grid.SelectionMode=DataGridViewSelectionMode.FullRowSelect; grid.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill; grid.CellClick+=SelectRow; Controls.Add(grid);
            Controls.Add(new Label { Text="Название", AutoSize=true, Location=new Point(15,250)}); name.Location=new Point(140,245); name.Size=new Size(785,25); name.MaxLength=150; Controls.Add(name);
            Controls.Add(new Label { Text="Категория", AutoSize=true, Location=new Point(15,305)}); category.Location=new Point(140,300); category.Size=new Size(350,25); category.DropDownStyle=ComboBoxStyle.DropDownList; Controls.Add(category);
            Controls.Add(new Label { Text="Цена", AutoSize=true, Location=new Point(535,305)}); price.Location=new Point(640,300); price.Size=new Size(285,25); price.DecimalPlaces=2; price.Maximum=999999999.99M; price.ThousandsSeparator=true; Controls.Add(price);
            Controls.Add(new Label { Text="Остаток", AutoSize=true, Location=new Point(15,365)}); stock.Location=new Point(140,360); stock.Size=new Size(350,25); stock.Maximum=1000000; Controls.Add(stock);
            AddButton("Добавить",15,Add); AddButton("Сохранить",245,Save); AddButton("Удалить",475,Delete); AddButton("Обновить",705,(s,e)=>{ LoadCategories(); LoadProducts(); ClearEditor(); },220); Load += (s,e)=>{ LoadCategories(); LoadProducts(); ClearEditor(); };
        }
        private void AddButton(string text,int x,EventHandler h,int width=210) { var b=new Button{Text=text,Location=new Point(x,460),Size=new Size(width,40)}; b.Click+=h; Controls.Add(b); }
        private void LoadCategories() { category.DisplayMember="name"; category.ValueMember="id"; category.DataSource=Db.Query("SELECT id,name FROM categories ORDER BY name"); category.SelectedIndex=-1; }
        private void LoadProducts() { grid.DataSource=Db.Query("SELECT p.id AS ID,p.name AS Товар,p.price AS Цена,p.stock AS Остаток,p.category_id AS CategoryID,c.name AS Категория FROM products p JOIN categories c ON c.id=p.category_id ORDER BY p.name"); grid.Columns["CategoryID"].Visible=false; grid.Columns["Цена"].DefaultCellStyle.Format="N2"; grid.ClearSelection(); selectedId=0; }
        private void ClearEditor() { selectedId=0; name.Clear(); category.SelectedIndex=-1; price.Value=0; stock.Value=0; grid.ClearSelection(); }
        private void SelectRow(object s,DataGridViewCellEventArgs e) { if(e.RowIndex<0)return; var r=grid.Rows[e.RowIndex]; selectedId=Convert.ToInt32(r.Cells["ID"].Value); name.Text=Convert.ToString(r.Cells["Товар"].Value); category.SelectedValue=Convert.ToInt32(r.Cells["CategoryID"].Value); price.Value=Convert.ToDecimal(r.Cells["Цена"].Value); stock.Value=Convert.ToDecimal(r.Cells["Остаток"].Value); }
        private bool Valid(){if(name.Text.Trim().Length==0){MessageBox.Show("Введите название товара.");return false;}if(category.SelectedIndex<0){MessageBox.Show("Выберите категорию.");return false;}return true;}
        private MySqlParameter[] Params() => new[]{new MySqlParameter("@name",name.Text.Trim()),new MySqlParameter("@category",Convert.ToInt32(category.SelectedValue)),new MySqlParameter("@price",price.Value),new MySqlParameter("@stock",Convert.ToInt32(stock.Value))};
        private void Add(object s,EventArgs e){if(!Valid())return;try{Db.Execute("INSERT INTO products(name,category_id,price,stock) VALUES(@name,@category,@price,@stock)",Params());LoadProducts();ClearEditor();}catch(Exception ex){Db.ShowError(ex);}}
        private void Save(object s,EventArgs e){if(selectedId==0){MessageBox.Show("Выберите товар в таблице.");return;}if(!Valid())return;try{var p=Params();Db.Execute("UPDATE products SET name=@name,category_id=@category,price=@price,stock=@stock WHERE id=@id",p[0],p[1],p[2],p[3],new MySqlParameter("@id",selectedId));LoadProducts();ClearEditor();}catch(Exception ex){Db.ShowError(ex);}}
        private void Delete(object s,EventArgs e){if(selectedId==0){MessageBox.Show("Выберите товар в таблице.");return;}if(MessageBox.Show("Удалить товар?","Подтверждение",MessageBoxButtons.YesNo)!=DialogResult.Yes)return;try{Db.Execute("DELETE FROM products WHERE id=@id",new MySqlParameter("@id",selectedId));LoadProducts();ClearEditor();}catch(Exception ex){Db.ShowError(ex);}}
    }
}
