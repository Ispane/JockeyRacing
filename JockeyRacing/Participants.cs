using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace JockeyRacing
{
    public partial class Participants : Form
    {
        DataBase dataBase = new DataBase();
        int selectedRow;

        public Participants()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void CreateColumns()
        {
            dataGridView1.Columns.Add("ID", "id");
            dataGridView1.Columns.Add("Jockey", "Жокей");
            dataGridView1.Columns.Add("New", String.Empty);
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord rcord)
        {
            dgw.Rows.Add(rcord.GetInt32(0), rcord.GetInt32(1), RowState.ModifiedNew);
        }

        private void RefreshData(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string querystring = $"select * from Participants";

            SqlCommand command = new SqlCommand(querystring, dataBase.getConnection());

            dataBase.openConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }

        private void Participants_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshData(dataGridView1);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            RefreshData(dataGridView1);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBox9.Text = row.Cells[0].Value.ToString();
                textBox1.Text = row.Cells[1].Value.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataBase.openConnection();

            int id;
            int Jockey;

            if (int.TryParse(textBox9.Text, out id))
            {
                if (int.TryParse(textBox1.Text, out Jockey))
                {
                    var addQuery = $"insert into Participants(ID, Jockey) values" +
                               $"('{id}','{Jockey}')";

                    var command = new SqlCommand(addQuery, dataBase.getConnection());
                    command.ExecuteNonQuery();

                    MessageBox.Show("Запись добавленна", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);                   
                }
                else
                {
                    MessageBox.Show("Запись не удалось добавить", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            dataBase.closeConnection();

        }

        private void deleteRow()
        {
            int i = dataGridView1.CurrentCell.RowIndex;

            dataGridView1.Rows[i].Visible = false;

            if (dataGridView1.Rows[i].Cells[0].Value.ToString() == string.Empty)
            {
                dataGridView1.Rows[i].Cells[2].Value = RowState.Deleted;
                return;
            }
            dataGridView1.Rows[i].Cells[2].Value = RowState.Deleted;
        }

        private void UpdateDGW()
        {
            dataBase.openConnection();

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                string rowState = Convert.ToString(dataGridView1.Rows[i].Cells[2].Value);

                if (rowState == "Existed")
                {
                    continue;
                }

                if (rowState == "Deleted")
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value);
                    var deleteQuery = $"delete from Participants where id = {id}";

                    var command = new SqlCommand(deleteQuery, dataBase.getConnection());
                    command.ExecuteNonQuery();
                }

                if (rowState == "Modified")
                {
                    var id = dataGridView1.Rows[i].Cells[0].Value.ToString();
                    var jockey = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    
                    var changeQuery = $"update Participants set Jockey = '{jockey}'" +
                        $" where ID = '{id}'";

                    var command = new SqlCommand(changeQuery, dataBase.getConnection());
                    command.ExecuteNonQuery();
                }
            }

            dataBase.closeConnection();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            deleteRow();
        }

        private void Search(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string searchString = $"select * from  Participants where concat (Jockey)" +
                $"like '%" + textBox8.Text + "%'";

            SqlCommand com = new SqlCommand(searchString, dataBase.getConnection());

            dataBase.openConnection();

            SqlDataReader read = com.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();

            dataBase.closeConnection();
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            UpdateDGW();
        }

        private void Change()
        {
            var selectedRowI = dataGridView1.CurrentCell.RowIndex;

            int id;
            int Jockey;

            if (dataGridView1.Rows[selectedRowI].Cells[0].Value.ToString() != string.Empty)
            {
                if (int.TryParse(textBox9.Text, out id))
                {
                    if (int.TryParse(textBox1.Text, out Jockey))
                    {
                        dataGridView1.Rows[selectedRowI].SetValues(id, Jockey);
                        dataGridView1.Rows[selectedRowI].Cells[2].Value = RowState.Modified;
                    }
                    else
                    {
                        MessageBox.Show("Запись неверная", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }                 
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Change();
        }

        private void label9_Click(object sender, EventArgs e)
        {
            Racing form = new Racing();
            this.Hide();
            form.Show();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Horses form = new Horses();
            this.Hide();
            form.Show();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            Jockeys form = new Jockeys();
            this.Hide();
            form.Show();
        }

        private void label12_Click(object sender, EventArgs e)
        {
            Racetracks form = new Racetracks();
            this.Hide();
            form.Show();
        }
    }
}
