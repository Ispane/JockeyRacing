using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace JockeyRacing
{
    public partial class Jockeys : Form
    {
        DataBase dataBase = new DataBase();
        int selectedRow;

        public Jockeys()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }


        private void CreateColumns()
        {
            dataGridView1.Columns.Add("ID", "id");
            dataGridView1.Columns.Add("FIO", "ФИО");
            dataGridView1.Columns.Add("Starts", "Стартов");
            dataGridView1.Columns.Add("Number_of_prizes", "Призовых мест");
            dataGridView1.Columns.Add("Horse", "Лошадь");
            dataGridView1.Columns.Add("New", String.Empty);
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord rcord)
        {
            dgw.Rows.Add(rcord.GetInt32(0), rcord.GetString(1), rcord.GetInt32(2),
                rcord.GetInt32(3), rcord.GetInt32(4), RowState.ModifiedNew);
        }

        private void RefreshData(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string querystring = $"select * from Jockeys";

            SqlCommand command = new SqlCommand(querystring, dataBase.getConnection());

            dataBase.openConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }

        private void Jockeys_Load(object sender, EventArgs e)
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
                textBox2.Text = row.Cells[2].Value.ToString();
                textBox3.Text = row.Cells[3].Value.ToString();
                textBox4.Text = row.Cells[4].Value.ToString();             
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataBase.openConnection();

            int id;
            var FIO = textBox1.Text;
            int Starts;
            int Number_of_prizes;
            int Horse;

            if (int.TryParse(textBox9.Text, out id))
            {
                if (int.TryParse(textBox2.Text, out Number_of_prizes))
                {
                    if (int.TryParse(textBox4.Text, out Horse))
                    {
                        if (int.TryParse(textBox3.Text, out Starts))
                        {
                            var addQuery = $"insert into Jockeys(ID, FIO, Starts, Number_of_prizes, Horse) values" +
                                $"('{id}','{FIO}', '{Starts}', '{Number_of_prizes}', '{Horse}')";

                            var command = new SqlCommand(addQuery, dataBase.getConnection());
                            command.ExecuteNonQuery();

                            MessageBox.Show("Запись добавленна", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Запись не удалось добавить", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        dataBase.closeConnection();
                        
                    }
                }
            }
        }

        private void deleteRow()
        {
            int i = dataGridView1.CurrentCell.RowIndex;

            dataGridView1.Rows[i].Visible = false;

            if (dataGridView1.Rows[i].Cells[0].Value.ToString() == string.Empty)
            {
                dataGridView1.Rows[i].Cells[5].Value = RowState.Deleted;
                return;
            }
            dataGridView1.Rows[i].Cells[5].Value = RowState.Deleted;
        }

        private void UpdateDGW()
        {
            dataBase.openConnection();

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                string rowState = Convert.ToString(dataGridView1.Rows[i].Cells[5].Value);

                if (rowState == "Existed")
                {
                    continue;
                }

                if (rowState == "Deleted")
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value);
                    var deleteQuery = $"delete from Jockeys where id = {id}";

                    var command = new SqlCommand(deleteQuery, dataBase.getConnection());
                    command.ExecuteNonQuery();
                }

                if (rowState == "Modified")
                {
                    var id = dataGridView1.Rows[i].Cells[0].Value.ToString();
                    var fio = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    var starts = dataGridView1.Rows[i].Cells[2].Value.ToString();
                    var number_of_prizes = dataGridView1.Rows[i].Cells[3].Value.ToString();
                    var horse = dataGridView1.Rows[i].Cells[4].Value.ToString();
                    
                    var changeQuery = $"update Jockeys set FIO = '{fio}', Starts = '{starts}', Number_of_prizes = '{number_of_prizes}'," +
                        $" Horse = '{horse}' where ID = '{id}'";

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

            string searchString = $"select * from Jockeys where concat (FIO, Starts, Number_of_prizes, Horse)" +
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
            var FIO = textBox1.Text;
            int Starts;
            int Number_of_prizes;
            int Horse;

            

            if (dataGridView1.Rows[selectedRowI].Cells[0].Value.ToString() != string.Empty)
            {
                if (int.TryParse(textBox9.Text, out id))
                {
                    if (int.TryParse(textBox2.Text, out Number_of_prizes))
                    {
                        if (int.TryParse(textBox4.Text, out Horse))
                        {
                            if (int.TryParse(textBox3.Text, out Starts))
                            {
                                dataGridView1.Rows[selectedRowI].SetValues(id, FIO, Starts, Number_of_prizes, Horse);
                                    dataGridView1.Rows[selectedRowI].Cells[5].Value = RowState.Modified;
                            }
                            else
                            {
                                    MessageBox.Show("Запись неверная", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }                           
                        }
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
            Racetracks form = new Racetracks();
            this.Hide();
            form.Show();
        }

        private void label12_Click(object sender, EventArgs e)
        {
            Participants form = new Participants();
            this.Hide();
            form.Show();
        }
    }
}

