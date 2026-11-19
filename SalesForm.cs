using System;
using System.Drawing;
using System.Windows.Forms;
using MySqlConnector;

namespace ComputerStore
{
    public sealed class SalesForm : Form
    {
        private readonly ComboBox product = new ComboBox(); private readonly NumericUpDown quantity = new NumericUpDown(); private readonly DataGridView grid = new DataGridView();
        public SalesForm()
        {
            Text="Продажи"; ClientSize=new Size(940,550); StartPosition=FormStartPosition.CenterParent; FormBorderStyle=FormBorderStyle.FixedSingle; MaximizeBox=false;
            Controls.Add(new Label{Text="Товар",AutoSize=true,Location=new Point(15,30)}); product.Location=new Point(145,25); product.Size=new Size(780,25); product.DropDownStyle=ComboBoxStyle.DropDownList; Controls.Add(product);
            Controls.Add(new Label{Text="Количество",AutoSize=true,Location=new Point(15,90)}); quantity.Location=new Point(145,85); quantity.Size=new Size(180,25); quantity.Minimum=1; quantity.Maximum=1000000; quantity.Value=1; Controls.Add(quantity);
            AddButton("Оформить продажу",355,Sell); AddButton("Обновить",655,(s,e)=>RefreshData());
            grid.Location=new Point(15,180); grid.Size=new Size(910,345); grid.ReadOnly=true; grid.AllowUserToAddRows=grid.AllowUserToDeleteRows=false; grid.MultiSelect=false; grid.SelectionMode=DataGridViewSelectionMode.FullRowSelect; grid.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill; Controls.Add(grid); Load += (s,e)=>RefreshData();
        }
        private void AddButton(string text,int x,EventHandler h){var b=new Button{Text=text,Location=new Point(x,80),Size=new Size(270,40)};b.Click+=h;Controls.Add(b);}
        private void LoadProducts(){product.DisplayMember="title";product.ValueMember="id";product.DataSource=Db.Query("SELECT id, CONCAT(name, ' | остаток: ', stock) AS title FROM products WHERE stock>0 ORDER BY name");product.SelectedIndex=-1;}
        private void LoadSales(){grid.DataSource=Db.Query("SELECT s.id AS ID,s.sold_at AS Дата,p.name AS Товар,s.quantity AS Количество,s.unit_price AS Цена,s.quantity*s.unit_price AS Сумма FROM sales s JOIN products p ON p.id=s.product_id ORDER BY s.id DESC");grid.Columns["Дата"].DefaultCellStyle.Format="dd.MM.yyyy HH:mm";grid.Columns["Цена"].DefaultCellStyle.Format="N2";grid.Columns["Сумма"].DefaultCellStyle.Format="N2";grid.ClearSelection();}
        private void RefreshData(){try{LoadProducts();LoadSales();quantity.Value=1;}catch(Exception ex){Db.ShowError(ex);}}
        private void Sell(object sender,EventArgs e)
        {
            if(product.SelectedIndex<0){MessageBox.Show("Выберите товар.");return;} int id=Convert.ToInt32(product.SelectedValue); int count=Convert.ToInt32(quantity.Value);
            try { MakeSale(id,count); MessageBox.Show("Продажа сохранена."); RefreshData(); }
            catch(Exception ex){Db.ShowError(ex);}
        }
        private static void MakeSale(int productId,int count)
        {
            using(var connection=Db.Open()) using(var transaction=connection.BeginTransaction())
            {
                using(var command=new MySqlCommand("UPDATE products SET stock=stock-@quantity WHERE id=@id AND stock>=@quantity",connection,transaction))
                {command.Parameters.AddWithValue("@id",productId);command.Parameters.AddWithValue("@quantity",count);if(command.ExecuteNonQuery()!=1)throw new InvalidOperationException("Недостаточно товара или товар удален.");}
                using(var command=new MySqlCommand("INSERT INTO sales(product_id,quantity,unit_price) SELECT id,@quantity,price FROM products WHERE id=@id",connection,transaction))
                {command.Parameters.AddWithValue("@id",productId);command.Parameters.AddWithValue("@quantity",count);if(command.ExecuteNonQuery()!=1)throw new InvalidOperationException("Не удалось записать продажу.");}
                transaction.Commit();
            }
        }
    }
}
