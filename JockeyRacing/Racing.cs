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
    enum RowState
    {
        Existed,
        New,
        Modified,
        ModifiedNew,
        Deleted
    }

    public partial class Racing : Form
    {
        DataBase dataBase = new DataBase();
        int selectedRow;

        public Racing()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }
        private void CreateColumns()
        {
            dataGridView1.Columns.Add("ID", "id");
            dataGridView1.Columns.Add("Name", "Название скачек");
            dataGridView1.Columns.Add("Racetrack", "Ипподром");
            dataGridView1.Columns.Add("Participants_Count", "Количество участников");
            dataGridView1.Columns.Add("Participants", "Участники");
            dataGridView1.Columns.Add("Prize", "Призовой фонд");
            dataGridView1.Columns.Add("Date", "Дата проведения");
            dataGridView1.Columns.Add("Time", "Время проведения");
            dataGridView1.Columns.Add("New", String.Empty);
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord rcord) 
        {
            dgw.Rows.Add(rcord.GetInt32(0), rcord.GetString(1), rcord.GetInt32(2),
                rcord.GetInt32(3), rcord.GetInt32(4), rcord.GetInt32(5), rcord.GetString(6),
                rcord.GetString(7), RowState.ModifiedNew);
        }

        private void RefreshData(DataGridView dgw) 
        {
            dgw.Rows.Clear();

            string querystring = $"select * from horseRacing";

            SqlCommand command = new SqlCommand(querystring, dataBase.getConnection());

            dataBase.openConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read()) 
            { 
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }

        private void Racing_Load(object sender, EventArgs e)
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

            if(e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBox9.Text = row.Cells[0].Value.ToString();
                textBox1.Text = row.Cells[1].Value.ToString();
                textBox2.Text = row.Cells[2].Value.ToString();
                textBox3.Text = row.Cells[3].Value.ToString();
                textBox4.Text = row.Cells[4].Value.ToString();
                textBox5.Text = row.Cells[5].Value.ToString();
                textBox6.Text = row.Cells[6].Value.ToString();
                textBox7.Text = row.Cells[7].Value.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataBase.openConnection();

            int id;
            var name = textBox1.Text;
            int ippodrom;
            int countParticipants;
            int participant;
            int prize;
            var date = textBox6.Text;
            var time = textBox7.Text;
            if (int.TryParse(textBox9.Text, out id))
            {
                if (int.TryParse(textBox2.Text, out ippodrom))
                {
                    if (int.TryParse(textBox3.Text, out countParticipants))
                    {
                        if (int.TryParse(textBox4.Text, out participant))
                        {
                            if (int.TryParse(textBox5.Text, out prize))
                            {
                                var addQuery = $"insert into horseRacing(ID, Name, Racetrack, Participants_Count, Participants, Prize, Date, Time) values" +
                                    $"('{id}','{name}', '{ippodrom}', '{countParticipants}', '{participant}', '{prize}', '{date}', '{time}')";

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
        }

        private void deleteRow()
        {
            int i = dataGridView1.CurrentCell.RowIndex;

            dataGridView1.Rows[i].Visible = false;

            if (dataGridView1.Rows[i].Cells[0].Value.ToString() == string.Empty)
            {
                dataGridView1.Rows[i].Cells[8].Value = RowState.Deleted;
                return;
            }
            dataGridView1.Rows[i].Cells[8].Value = RowState.Deleted;
        }

        private void UpdateDGW()
        {
            dataBase.openConnection();

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                string rowState = Convert.ToString(dataGridView1.Rows[i].Cells[8].Value);

                if (rowState == "Existed")
                {
                    continue;
                }

                if (rowState == "Deleted")
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value);
                    var deleteQuery = $"delete from horseRacing where id = {id}";

                    var command = new SqlCommand(deleteQuery, dataBase.getConnection());
                    command.ExecuteNonQuery();
                }

                if (rowState == "Modified")
                {
                    var id = dataGridView1.Rows[i].Cells[0].Value.ToString();
                    var name = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    var ippodrom = dataGridView1.Rows[i].Cells[2].Value.ToString();
                    var countParticipants = dataGridView1.Rows[i].Cells[3].Value.ToString();
                    var participant = dataGridView1.Rows[i].Cells[4].Value.ToString();
                    var prize = dataGridView1.Rows[i].Cells[5].Value.ToString();
                    var date = dataGridView1.Rows[i].Cells[6].Value.ToString();
                    var time = dataGridView1.Rows[i].Cells[7].Value.ToString();

                    var changeQuery = $"update horseRacing set Name = '{name}', Racetrack = '{ippodrom}', Participants_Count = '{countParticipants}'," +
                        $" Participants = '{participant}', Prize = '{prize}', Date = '{date}', Time = '{time}' where ID = '{id}'";

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

            string searchString = $"select * from horseRacing where concat (Name, Racetrack, Participants_Count, Participants, Prize, Date, Time)" +
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
            var name = textBox1.Text;
            int ippodrom;
            int countParticipants;
            int participant;
            int prize;
            var date = textBox6.Text;
            var time = textBox7.Text;

            if (dataGridView1.Rows[selectedRowI].Cells[0].Value.ToString() != string.Empty ) 
            {
                if (int.TryParse(textBox9.Text, out id))
                {
                    if (int.TryParse(textBox2.Text, out ippodrom))
                    {
                        if (int.TryParse(textBox3.Text, out countParticipants))
                        {
                            if (int.TryParse(textBox4.Text, out participant))
                            {
                                if (int.TryParse(textBox5.Text, out prize))
                                {
                                    dataGridView1.Rows[selectedRowI].SetValues(id, name, ippodrom, countParticipants,
                                        participant, prize, date, time);
                                    dataGridView1.Rows[selectedRowI].Cells[8].Value = RowState.Modified;
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
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Change();
        }

        private void label9_Click(object sender, EventArgs e)
        {
            Horses form = new Horses();
            this.Hide();
            form.Show();
        }

        private void label10_Click(object sender, EventArgs e)
        {
            Jockeys form = new Jockeys();
            this.Hide();
            form.Show();
        }

        private void label11_Click(object sender, EventArgs e)
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
