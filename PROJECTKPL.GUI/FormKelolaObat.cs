using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace PROJECTKPL.GUI
{
    public class FormKelolaObat : Form
    {
        private Label lblJudul;
        private DataGridView dgvObat;
        private Panel pnlAksi;
        private TextBox txtNama;
        private TextBox txtStok;
        private TextBox txtHarga;
        private Button btnTambah;
        private Button btnEditStok;
        private Button btnHapus;
        private Button btnRefresh;
        private Label lblStatus;

        private readonly HttpClient _http;
        private List<JsonElement> _daftarObat = new();

        public FormKelolaObat(HttpClient http)
        {
            _http = http;
            InitializeComponent();
            _ = LoadObatAsync();
        }

        private void InitializeComponent()
        {
            Text = "Kelola Obat";
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9f);

            // ── Judul ─────────────────────────────────────────────────────
            lblJudul = new Label
            {
                Text = "Kelola Obat",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // ── DataGridView ──────────────────────────────────────────────
            dgvObat = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(660, 280),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9f),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvObat.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvObat.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(71, 85, 105);
            dgvObat.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvObat.EnableHeadersVisualStyles = false;
            dgvObat.Columns.Add("Id", "ID");
            dgvObat.Columns.Add("NamaObat", "Nama Obat");
            dgvObat.Columns.Add("Stok", "Stok");
            dgvObat.Columns.Add("Status", "Status");
            dgvObat.Columns.Add("Harga", "Harga");
            dgvObat.Columns["Id"].FillWeight = 40;
            dgvObat.Columns["Stok"].FillWeight = 60;
            dgvObat.Columns["Status"].FillWeight = 80;
            dgvObat.Columns["Harga"].FillWeight = 80;

            // ── Panel Aksi ────────────────────────────────────────────────
            pnlAksi = new Panel
            {
                Location = new Point(20, 355),
                Size = new Size(660, 180),
                BackColor = Color.White
            };
            pnlAksi.Paint += (s, e) =>
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(226, 232, 240)), 0, 0, pnlAksi.Width - 1, pnlAksi.Height - 1);

            var lblForm = new Label
            {
                Text = "Tambah / Edit Obat",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                Location = new Point(15, 12),
                AutoSize = true
            };

            var lblNama = BuatLabel("Nama Obat", 40);
            var lblStok = BuatLabel("Stok", 40);
            var lblHarga = BuatLabel("Harga (Rp)", 40);

            txtNama = BuatTextBox(15, 55, 200);
            txtStok = BuatTextBox(225, 55, 100);
            txtHarga = BuatTextBox(335, 55, 120);

            lblNama.Location = new Point(15, 38);
            lblStok.Location = new Point(225, 38);
            lblHarga.Location = new Point(335, 38);

            btnTambah = BuatTombolAksi("Tambah", 15, 100, Color.FromArgb(37, 99, 235));
            btnTambah.Click += BtnTambah_Click;

            btnEditStok = BuatTombolAksi("Edit Stok", 115, 100, Color.FromArgb(234, 179, 8));
            btnEditStok.Click += BtnEditStok_Click;

            btnHapus = BuatTombolAksi("Hapus", 215, 100, Color.FromArgb(220, 38, 38));
            btnHapus.Click += BtnHapus_Click;

            btnRefresh = BuatTombolAksi("Refresh", 315, 100, Color.FromArgb(100, 116, 139));
            btnRefresh.Click += async (s, e) => await LoadObatAsync();

            pnlAksi.Controls.AddRange(new Control[]
            {
                lblForm, lblNama, lblStok, lblHarga,
                txtNama, txtStok, txtHarga,
                btnTambah, btnEditStok, btnHapus, btnRefresh
            });

            // ── Status ────────────────────────────────────────────────────
            lblStatus = new Label
            {
                Text = "",
                Location = new Point(20, 545),
                AutoSize = true,
                ForeColor = Color.FromArgb(100, 116, 139),
                Font = new Font("Segoe UI", 8.5f)
            };

            Controls.AddRange(new Control[] { lblJudul, dgvObat, pnlAksi, lblStatus });
        }

        private Label BuatLabel(string text, int top) => new Label
        {
            Text = text,
            AutoSize = true,
            ForeColor = Color.FromArgb(71, 85, 105),
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            Location = new Point(0, top)
        };

        private TextBox BuatTextBox(int left, int top, int width) => new TextBox
        {
            Location = new Point(left, top),
            Size = new Size(width, 26),
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Segoe UI", 9f)
        };

        private Button BuatTombolAksi(string text, int left, int top, Color warna) => new Button
        {
            Text = text,
            Location = new Point(left, top),
            Size = new Size(90, 32),
            FlatStyle = FlatStyle.Flat,
            BackColor = warna,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        // ── Load data ──────────────────────────────────────────────────────
        private async Task LoadObatAsync()
        {
            try
            {
                var res = await _http.GetAsync("api/obat");
                if (!res.IsSuccessStatusCode) return;

                _daftarObat = await res.Content.ReadFromJsonAsync<List<JsonElement>>() ?? new();
                dgvObat.Rows.Clear();

                foreach (var o in _daftarObat)
                {
                    dgvObat.Rows.Add(
                        o.GetProperty("id"),
                        o.GetProperty("namaObat").GetString(),
                        o.GetProperty("stok"),
                        o.GetProperty("statusObat").GetString(),
                        $"Rp{o.GetProperty("harga")}"
                    );
                }
                lblStatus.Text = $"Total: {_daftarObat.Count} obat";
            }
            catch
            {
                lblStatus.Text = "Gagal memuat data.";
            }
        }

        // ── Tambah obat ────────────────────────────────────────────────────
        private async void BtnTambah_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNama.Text))
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Nama obat harus diisi.";
                return;
            }
            if (!int.TryParse(txtStok.Text, out int stok) || stok < 0)
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Stok harus berupa angka >= 0.";
                return;
            }
            if (!int.TryParse(txtHarga.Text, out int harga) || harga <= 0)
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Harga harus berupa angka > 0.";
                return;
            }

            try
            {
                var res = await _http.PostAsJsonAsync("api/obat",
                    new { namaObat = txtNama.Text.Trim(), stok, harga });

                if (res.IsSuccessStatusCode)
                {
                    lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
                    lblStatus.Text = "Obat berhasil ditambahkan.";
                    txtNama.Clear(); txtStok.Clear(); txtHarga.Clear();
                    await LoadObatAsync();
                }
                else
                {
                    string msg = await res.Content.ReadAsStringAsync();
                    lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                    lblStatus.Text = $"Gagal: {msg}";
                }
            }
            catch
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Tidak dapat terhubung ke server.";
            }
        }

        // ── Edit stok ──────────────────────────────────────────────────────
        private async void BtnEditStok_Click(object? sender, EventArgs e)
        {
            if (dgvObat.CurrentRow == null) return;
            if (!int.TryParse(txtStok.Text, out int stokBaru) || stokBaru < 0)
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Stok baru harus berupa angka >= 0.";
                return;
            }

            int id = (int)dgvObat.CurrentRow.Cells["Id"].Value;
            try
            {
                var res = await _http.PutAsJsonAsync($"api/obat/{id}/stok", new { stokBaru });
                if (res.IsSuccessStatusCode)
                {
                    lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
                    lblStatus.Text = "Stok berhasil diperbarui.";
                    await LoadObatAsync();
                }
                else
                {
                    lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                    lblStatus.Text = "Gagal memperbarui stok.";
                }
            }
            catch
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Tidak dapat terhubung ke server.";
            }
        }

        // ── Hapus obat ─────────────────────────────────────────────────────
        private async void BtnHapus_Click(object? sender, EventArgs e)
        {
            if (dgvObat.CurrentRow == null) return;

            string namaObat = dgvObat.CurrentRow.Cells["NamaObat"].Value?.ToString() ?? "";
            if (MessageBox.Show($"Hapus obat '{namaObat}'?", "Konfirmasi",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            int id = (int)dgvObat.CurrentRow.Cells["Id"].Value;
            try
            {
                var res = await _http.DeleteAsync($"api/obat/{id}");
                if (res.IsSuccessStatusCode)
                {
                    lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
                    lblStatus.Text = $"Obat '{namaObat}' berhasil dihapus.";
                    await LoadObatAsync();
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
