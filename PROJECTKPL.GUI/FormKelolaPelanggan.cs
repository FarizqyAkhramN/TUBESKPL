using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace PROJECTKPL.GUI
{
    public class FormKelolaPelanggan : Form
    {
        private Label lblJudul;
        private DataGridView dgvPelanggan;
        private Panel pnlAksi;
        private TextBox txtUsername;
        private TextBox txtGender;
        private TextBox txtNoTelp;
        private TextBox txtUmur;
        private TextBox txtPassword;
        private Button btnTambah;
        private Button btnHapus;
        private Button btnRefresh;
        private Label lblStatus;

        private readonly HttpClient _http;
        private List<JsonElement> _daftarPelanggan = new();

        public FormKelolaPelanggan(HttpClient http)
        {
            _http = http;
            InitializeComponent();
            _ = LoadPelangganAsync();
        }

        private void InitializeComponent()
        {
            Text = "Kelola Pelanggan";
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9f);

            lblJudul = new Label
            {
                Text = "Kelola Pelanggan",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 20),
                AutoSize = true
            };

            dgvPelanggan = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(660, 240),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9f),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvPelanggan.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvPelanggan.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(71, 85, 105);
            dgvPelanggan.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvPelanggan.EnableHeadersVisualStyles = false;
            dgvPelanggan.Columns.Add("Id", "ID");
            dgvPelanggan.Columns.Add("Username", "Username");
            dgvPelanggan.Columns.Add("Gender", "Gender");
            dgvPelanggan.Columns.Add("NoTelp", "No. Telepon");
            dgvPelanggan.Columns.Add("Umur", "Umur");
            dgvPelanggan.Columns["Id"].FillWeight = 30;

            pnlAksi = new Panel
            {
                Location = new Point(20, 315),
                Size = new Size(660, 210),
                BackColor = Color.White
            };
            pnlAksi.Paint += (s, e) =>
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(226, 232, 240)), 0, 0, pnlAksi.Width - 1, pnlAksi.Height - 1);

            var lblForm = new Label
            {
                Text = "Tambah Pelanggan",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                Location = new Point(15, 12),
                AutoSize = true
            };

            // Row 1
            var lblUsr = BuatLabel("Username", new Point(15, 38));
            var lblGen = BuatLabel("Gender", new Point(175, 38));
            var lblTlp = BuatLabel("No. Telepon", new Point(335, 38));
            txtUsername = BuatTextBox(new Point(15, 55), 150);
            txtGender = BuatTextBox(new Point(175, 55), 150);
            txtNoTelp = BuatTextBox(new Point(335, 55), 150);

            // Row 2
            var lblUmr = BuatLabel("Umur", new Point(15, 90));
            var lblPass = BuatLabel("Password", new Point(175, 90));
            txtUmur = BuatTextBox(new Point(15, 107), 150);
            txtPassword = BuatTextBox(new Point(175, 107), 150);
            txtPassword.PasswordChar = '●';

            btnTambah = BuatTombol("Tambah", new Point(15, 150), Color.FromArgb(37, 99, 235));
            btnTambah.Click += BtnTambah_Click;

            btnHapus = BuatTombol("Hapus", new Point(115, 150), Color.FromArgb(220, 38, 38));
            btnHapus.Click += BtnHapus_Click;

            btnRefresh = BuatTombol("Refresh", new Point(215, 150), Color.FromArgb(100, 116, 139));
            btnRefresh.Click += async (s, e) => await LoadPelangganAsync();

            pnlAksi.Controls.AddRange(new Control[]
            {
                lblForm,
                lblUsr, lblGen, lblTlp,
                txtUsername, txtGender, txtNoTelp,
                lblUmr, lblPass,
                txtUmur, txtPassword,
                btnTambah, btnHapus, btnRefresh
            });

            lblStatus = new Label
            {
                Text = "",
                Location = new Point(20, 535),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f)
            };

            Controls.AddRange(new Control[] { lblJudul, dgvPelanggan, pnlAksi, lblStatus });
        }

        private Label BuatLabel(string text, Point loc) => new Label
        {
            Text = text,
            Location = loc,
            AutoSize = true,
            ForeColor = Color.FromArgb(71, 85, 105),
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
        };

        private TextBox BuatTextBox(Point loc, int width) => new TextBox
        {
            Location = loc,
            Size = new Size(width, 26),
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Segoe UI", 9f)
        };

        private Button BuatTombol(string text, Point loc, Color warna) => new Button
        {
            Text = text,
            Location = loc,
            Size = new Size(90, 32),
            FlatStyle = FlatStyle.Flat,
            BackColor = warna,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        private async Task LoadPelangganAsync()
        {
            try
            {
                var res = await _http.GetAsync("api/pelanggan");
                if (!res.IsSuccessStatusCode) return;

                _daftarPelanggan = await res.Content.ReadFromJsonAsync<List<JsonElement>>() ?? new();
                dgvPelanggan.Rows.Clear();

                foreach (var p in _daftarPelanggan)
                {
                    dgvPelanggan.Rows.Add(
                        p.GetProperty("id"),
                        p.GetProperty("username").GetString(),
                        p.GetProperty("gender").GetString(),
                        p.GetProperty("noTelp").GetString(),
                        p.GetProperty("umur")
                    );
                }
                lblStatus.ForeColor = Color.FromArgb(100, 116, 139);
                lblStatus.Text = $"Total: {_daftarPelanggan.Count} pelanggan";
            }
            catch
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Gagal memuat data.";
            }
        }

        private async void BtnTambah_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtGender.Text) ||
                string.IsNullOrWhiteSpace(txtNoTelp.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Semua field harus diisi.";
                return;
            }
            if (!int.TryParse(txtUmur.Text, out int umur) || umur < 1 || umur > 120)
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Umur harus antara 1-120.";
                return;
            }

            try
            {
                var res = await _http.PostAsJsonAsync("api/pelanggan", new
                {
                    username = txtUsername.Text.Trim(),
                    gender = txtGender.Text.Trim(),
                    noTelp = txtNoTelp.Text.Trim(),
                    umur,
                    password = txtPassword.Text
                });

                if (res.IsSuccessStatusCode)
                {
                    lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
                    lblStatus.Text = "Pelanggan berhasil ditambahkan.";
                    txtUsername.Clear(); txtGender.Clear();
                    txtNoTelp.Clear(); txtUmur.Clear(); txtPassword.Clear();
                    await LoadPelangganAsync();
                }
                else
                {
                    lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                    lblStatus.Text = "Gagal menambahkan pelanggan. Cek validasi.";
                }
            }
            catch
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Tidak dapat terhubung ke server.";
            }
        }

        private async void BtnHapus_Click(object? sender, EventArgs e)
        {
            if (dgvPelanggan.CurrentRow == null) return;

            string nama = dgvPelanggan.CurrentRow.Cells["Username"].Value?.ToString() ?? "";
            if (MessageBox.Show($"Hapus pelanggan '{nama}'?", "Konfirmasi",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            int id = (int)dgvPelanggan.CurrentRow.Cells["Id"].Value;
            try
            {
                var res = await _http.DeleteAsync($"api/pelanggan/{id}");
                if (res.IsSuccessStatusCode)
                {
                    lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
                    lblStatus.Text = $"Pelanggan '{nama}' berhasil dihapus.";
                    await LoadPelangganAsync();
                }
            }
            catch
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Tidak dapat terhubung ke server.";
            }
        }
    }
}
