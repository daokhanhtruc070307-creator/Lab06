using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyHocSinh
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormHocSinh());
        }
    }

    public class FormHocSinh : Form
    {
        private string chuoiketnoi = "Server=.; Database=DBHocSinh; Integrated Security=SSPI;";

        private Label lblMaHS, lblHoTen, lblMaLop;
        private TextBox txtMaHS, txtHoTen, txtMaLop;
        private ListView lvHocSinh;
        private Button btnThem, btnSua, btnXoa;

        public FormHocSinh()
        {
            this.Text = "Quan ly Hoc Sinh - Connected Model";
            this.Size = new Size(600, 480);
            this.StartPosition = FormStartPosition.CenterScreen;

            lblMaHS = new Label() { Text = "Ma HS:", Location = new Point(20, 20), AutoSize = true };
            txtMaHS = new TextBox() { Location = new Point(90, 17), Width = 150 };

            lblHoTen = new Label() { Text = "Ho Ten:", Location = new Point(20, 60), AutoSize = true };
            txtHoTen = new TextBox() { Location = new Point(90, 57), Width = 250 };

            lblMaLop = new Label() { Text = "Ma Lop:", Location = new Point(20, 100), AutoSize = true };
            txtMaLop = new TextBox() { Location = new Point(90, 97), Width = 150 };

            lvHocSinh = new ListView()
            {
                Location = new Point(20, 140),
                Width = 540,
                Height = 220,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            lvHocSinh.Columns.Add("Ma HS", 100);
            lvHocSinh.Columns.Add("Ho Ten", 250);
            lvHocSinh.Columns.Add("Ma Lop", 120);
            lvHocSinh.ItemSelectionChanged += lvHocSinh_ItemSelectionChanged;

            btnThem = new Button() { Text = "Them", Location = new Point(20, 380), Width = 80, Height = 30 };
            btnThem.Click += btnThem_Click;

            btnSua = new Button() { Text = "Sua", Location = new Point(110, 380), Width = 80, Height = 30 };
            btnSua.Click += btnSua_Click;

            btnXoa = new Button() { Text = "Xoa", Location = new Point(200, 380), Width = 80, Height = 30 };
            btnXoa.Click += btnXoa_Click;

            this.Controls.Add(lblMaHS);
            this.Controls.Add(txtMaHS);
            this.Controls.Add(lblHoTen);
            this.Controls.Add(txtHoTen);
            this.Controls.Add(lblMaLop);
            this.Controls.Add(txtMaLop);
            this.Controls.Add(lvHocSinh);
            this.Controls.Add(btnThem);
            this.Controls.Add(btnSua);
            this.Controls.Add(btnXoa);

            this.Load += (s, e) => LoadDanhSachHocSinh();
        }

        private void LoadDanhSachHocSinh()
        {
            lvHocSinh.Items.Clear();
            using (SqlConnection con = new SqlConnection(chuoiketnoi))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT MaHS, HoTen, MaLop FROM HocSinh", con);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string mahs = dr["MaHS"].ToString();
                        string hoten = dr["HoTen"].ToString();
                        string malop = dr["MaLop"].ToString();

                        ListViewItem lvi = new ListViewItem(mahs);
                        lvi.Tag = mahs;
                        lvi.SubItems.Add(hoten);
                        lvi.SubItems.Add(malop);

                        lvHocSinh.Items.Add(lvi);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Loi ket noi hoac truy van: " + ex.Message);
                }
            }
        }

        private void lvHocSinh_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                txtMaHS.Text = e.Item.Text;
                txtHoTen.Text = e.Item.SubItems[1].Text;
                txtMaLop.Text = e.Item.SubItems[2].Text;
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            string maHS = txtMaHS.Text.Trim();
            string hoTen = txtHoTen.Text.Trim();
            string maLop = txtMaLop.Text.Trim();

            if (string.IsNullOrEmpty(maHS))
            {
                MessageBox.Show("Vui long nhap Ma hoc sinh!");
                return;
            }

            using (SqlConnection con = new SqlConnection(chuoiketnoi))
            {
                try
                {
                    con.Open();
                    string sql = "INSERT INTO HocSinh(MaHS, HoTen, MaLop) VALUES (@MaHS, @HoTen, @MaLop)";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaHS", maHS);
                    cmd.Parameters.AddWithValue("@HoTen", hoTen);
                    cmd.Parameters.AddWithValue("@MaLop", maLop);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Them hoc sinh thanh cong!");
                    LoadDanhSachHocSinh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Loi khi them: " + ex.Message);
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (lvHocSinh.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui long chon hoc sinh can sua tren danh sach!");
                return;
            }

            string maHS = lvHocSinh.SelectedItems[0].Tag.ToString();
            string hoTen = txtHoTen.Text.Trim();
            string maLop = txtMaLop.Text.Trim();

            using (SqlConnection con = new SqlConnection(chuoiketnoi))
            {
                try
                {
                    con.Open();
                    string sql = "UPDATE HocSinh SET HoTen = @HoTen, MaLop = @MaLop WHERE MaHS = @MaHS";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaHS", maHS);
                    cmd.Parameters.AddWithValue("@HoTen", hoTen);
                    cmd.Parameters.AddWithValue("@MaLop", maLop);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cap nhat hoc sinh thanh cong!");
                    LoadDanhSachHocSinh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Loi khi sua: " + ex.Message);
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (lvHocSinh.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui long chon hoc sinh can xoa tren danh sach!");
                return;
            }

            string maHS = lvHocSinh.SelectedItems[0].Tag.ToString();

            DialogResult dr = MessageBox.Show($"Ban co chac chan muon xoa hoc sinh co ma {maHS}?", "Xac nhan", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(chuoiketnoi))
                {
                    try
                    {
                        con.Open();
                        string sql = "DELETE FROM HocSinh WHERE MaHS = @MaHS";
                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.Parameters.AddWithValue("@MaHS", maHS);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Xoa hoc sinh thanh cong!");
                        LoadDanhSachHocSinh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Loi khi xoa: " + ex.Message);
                    }
                }
            }
        }
    }
}