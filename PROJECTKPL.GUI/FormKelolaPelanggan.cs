using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROJECTKPL.GUI
{
    public class FormKelolaPelanggan : Form
    {
        private enum Mode
        {
            Add,
            View
        }

        private Mode currentMode = Mode.Add;

        private Label lblJudul;

        private DataGridView dgvPelanggan;
        private Panel pnlAksi;
        private Panel pnlStatus;

        private TextBox txtUsername;
        private ComboBox cmbGender;
        private TextBox txtNoTelp;
        private TextBox txtUmur;
        private TextBox txtPassword;

        private Button btnTambah;
        private Button btnHapus;
        private Button btnRefresh;

        private Label lblStatus;

        private readonly HttpClient _http;
        private List<JsonElement> _daftarPelanggan = new();

        private int? selectedPelangganId = null;

        public FormKelolaPelanggan(HttpClient http)
        {
            _http = http;
            InitializeComponent();
            _ = LoadPelangganAsync();
        }

        private void InitializeComponent()
        {
            Text = "Kelola Pelanggan";
            Size = new Size(720, 600);
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9f);

            // ================= TITLE =================
            lblJudul = new Label
            {
                Text = "Kelola Pelanggan",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 15),
                AutoSize = true
            };

            // ================= GRID =================
            dgvPelanggan = new DataGridView
            {
                Location = new Point(20, 55),
                Size = new Size(660, 240),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvPelanggan.Columns.Add("Id", "ID");
            dgvPelanggan.Columns.Add("Username", "Username");
            dgvPelanggan.Columns.Add("Gender", "Gender");
            dgvPelanggan.Columns.Add("NoTelp", "No. Telepon");
            dgvPelanggan.Columns.Add("Umur", "Umur");

            dgvPelanggan.CellClick += DgvPelanggan_CellClick;

            // ================= FORM PANEL =================
            pnlAksi = new Panel
            {
                Location = new Point(20, 320),
                Size = new Size(660, 200),
                BackColor = Color.White
            };

            pnlAksi.Paint += (s, e) =>
            {
                e.Graphics.DrawRectangle(
                    new Pen(Color.FromArgb(226, 232, 240)),
                    0, 0, pnlAksi.Width - 1, pnlAksi.Height - 1
                );
            };

            var lblForm = new Label
            {
                Text = "Tambah Pelanggan",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Location = new Point(15, 10),
                AutoSize = true
            };

            // ================= INPUT =================
            txtUsername = new TextBox { Location = new Point(15, 55), Size = new Size(150, 26) };

            cmbGender = new ComboBox
            {
                Location = new Point(175, 55),
                Size = new Size(150, 26),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbGender.Items.AddRange(new object[]
            {
                "Laki-laki",
                "Perempuan"
            });

            txtNoTelp = new TextBox { Location = new Point(335, 55), Size = new Size(150, 26) };

            txtUmur = new TextBox { Location = new Point(15, 110), Size = new Size(150, 26) };

            txtPassword = new TextBox
            {
                Location = new Point(175, 110),
                Size = new Size(150, 26),
                PasswordChar = '●'
            };

            // ================= LABELS =================
            var lbl1 = CreateLabel("Username", new Point(15, 35));
            var lbl2 = CreateLabel("Gender", new Point(175, 35));
            var lbl3 = CreateLabel("No Telp", new Point(335, 35));
            var lbl4 = CreateLabel("Umur", new Point(15, 90));
            var lbl5 = CreateLabel("Password", new Point(175, 90));

            // ================= BUTTON =================
            btnTambah = CreateButton("Tambah", new Point(15, 150), Color.FromArgb(37, 99, 235));
            btnHapus = CreateButton("Hapus", new Point(115, 150), Color.FromArgb(220, 38, 38));
            btnRefresh = CreateButton("Refresh", new Point(215, 150), Color.FromArgb(100, 116, 139));

            btnTambah.Click += BtnTambah_Click;
            btnHapus.Click += BtnHapus_Click;
            btnRefresh.Click += async (s, e) => await RefreshAll();

            pnlAksi.Controls.AddRange(new Control[]
            {
                lblForm,
                lbl1, lbl2, lbl3, lbl4, lbl5,
                txtUsername,
                cmbGender,
                txtNoTelp,
                txtUmur,
                txtPassword,
                btnTambah,
                btnHapus,
                btnRefresh
            });

            // ================= STATUS PANEL (FIX KEPOtONG) =================
            pnlStatus = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 30,
                BackColor = Color.White
            };

            lblStatus = new Label
            {
                Dock = DockStyle.Left,
                AutoSize = true,
                Padding = new Padding(10, 5, 0, 0),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            pnlStatus.Controls.Add(lblStatus);

            // ================= ADD TO FORM =================
            Controls.AddRange(new Control[]
            {
                lblJudul,
                dgvPelanggan,
                pnlAksi,
                pnlStatus
            });
        }

        // ================= GRID CLICK =================
        private void DgvPelanggan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvPelanggan.Rows[e.RowIndex];

            selectedPelangganId = Convert.ToInt32(row.Cells["Id"].Value);

            txtUsername.Text = row.Cells["Username"].Value?.ToString();
            txtNoTelp.Text = row.Cells["NoTelp"].Value?.ToString();
            txtUmur.Text = row.Cells["Umur"].Value?.ToString();

            string gender = row.Cells["Gender"].Value?.ToString();
            cmbGender.SelectedItem = gender;

            txtPassword.Clear();

            currentMode = Mode.View;
            lblStatus.Text = "MODE: VIEW";
        }

        // ================= LOAD =================
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
                        p.GetProperty("id").GetInt32(),
                        p.GetProperty("username").GetString(),
                        p.GetProperty("gender").GetString(),
                        p.GetProperty("noTelp").GetString(),
                        p.GetProperty("umur").GetInt32()
                    );
                }

                lblStatus.Text = $"Total: {_daftarPelanggan.Count} pelanggan | MODE: {currentMode}";
            }
            catch
            {
                lblStatus.Text = "Gagal load data.";
            }
        }

        // ================= TAMBAH =================
        private async void BtnTambah_Click(object sender, EventArgs e)
        {
            if (!Validate(out int umur)) return;

            var res = await _http.PostAsJsonAsync("api/pelanggan", new
            {
                username = txtUsername.Text.Trim(),
                gender = cmbGender.SelectedItem?.ToString(),
                noTelp = txtNoTelp.Text.Trim(),
                umur,
                password = txtPassword.Text
            });

            if (res.IsSuccessStatusCode)
            {
                lblStatus.Text = "Pelanggan berhasil ditambahkan.";
                await RefreshAll();
            }
        }

        // ================= DELETE =================
        private async void BtnHapus_Click(object sender, EventArgs e)
        {
            if (selectedPelangganId == null)
            {
                lblStatus.Text = "Pilih pelanggan dulu.";
                return;
            }

            await _http.DeleteAsync($"api/pelanggan/{selectedPelangganId}");

            lblStatus.Text = "Pelanggan dihapus.";

            await RefreshAll();
        }

        // ================= RESET =================
        private async Task RefreshAll()
        {
            ResetForm();
            await LoadPelangganAsync();
        }

        private void ResetForm()
        {
            txtUsername.Clear();
            txtNoTelp.Clear();
            txtUmur.Clear();
            txtPassword.Clear();

            cmbGender.SelectedIndex = -1;

            selectedPelangganId = null;
            currentMode = Mode.Add;

            dgvPelanggan.ClearSelection();

            lblStatus.Text = "MODE: ADD";
        }

        // ================= VALIDATION =================
        private bool Validate(out int umur)
        {
            umur = 0;

            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                cmbGender.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtNoTelp.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblStatus.Text = "Semua field wajib diisi.";
                return false;
            }

            if (!int.TryParse(txtUmur.Text, out umur) || umur < 1 || umur > 120)
            {
                lblStatus.Text = "Umur tidak valid.";
                return false;
            }

            return true;
        }

        // ================= HELPERS =================
        private Label CreateLabel(string text, Point loc)
        {
            return new Label
            {
                Text = text,
                Location = loc,
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105)
            };
        }

        private Button CreateButton(string text, Point loc, Color color)
        {
            return new Button
            {
                Text = text,
                Location = loc,
                Size = new Size(90, 32),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
        }
    }
}