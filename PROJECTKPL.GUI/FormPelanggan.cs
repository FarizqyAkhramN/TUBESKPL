using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using PROJECTKPL.GUI.Services;

namespace PROJECTKPL.GUI
{
    // ══════════════════════════════════════════════════════════════════════
    // FormDaftarObat — dengan fitur search & color formatting
    // ══════════════════════════════════════════════════════════════════════
    public class FormDaftarObat : Form
    {
        private Label lblJudul;
        private Label lblStatus;
        private TextBox txtCari;
        private Button btnRefresh;
        private DataGridView dgvObat;

        // Facade Pattern — pakai ObatService bukan HttpClient langsung
        private readonly ObatService _obatService;
        private List<JsonElement> _daftarObat = new();

        public FormDaftarObat(ObatService obatService)
        {
            _obatService = obatService;
            InitializeComponent();
            _ = LoadAsync();
        }

        private void InitializeComponent()
        {
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9f);

            lblJudul = new Label
            {
                Text = "Daftar Obat Tersedia",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 20),
                AutoSize = true
            };

            txtCari = new TextBox
            {
                Location = new Point(20, 60),
                Size = new Size(250, 28),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9f),
                PlaceholderText = "Cari nama obat..."
            };
            txtCari.TextChanged += TxtCari_TextChanged;

            btnRefresh = new Button
            {
                Text = "↻ Refresh",
                Location = new Point(560, 58),
                Size = new Size(110, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 116, 139),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += async (s, e) =>
            {
                txtCari.Clear();
                await LoadAsync();
            };

            dgvObat = new DataGridView
            {
                Location = new Point(20, 105),
                Size = new Size(650, 360),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
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

            dgvObat.Columns["Id"].Visible = false;
            dgvObat.Columns["NamaObat"].FillWeight = 180;
            dgvObat.Columns["Stok"].FillWeight = 60;
            dgvObat.Columns["Status"].FillWeight = 100;
            dgvObat.Columns["Harga"].FillWeight = 100;

            dgvObat.CellFormatting += DgvObat_CellFormatting;

            lblStatus = new Label
            {
                Text = "Memuat data...",
                Location = new Point(20, 480),
                Size = new Size(650, 30),
                ForeColor = Color.FromArgb(100, 116, 139),
                Font = new Font("Segoe UI", 8.5f)
            };

            Controls.AddRange(new Control[] { lblJudul, txtCari, btnRefresh, dgvObat, lblStatus });
        }

        private async Task LoadAsync()
        {
            try
            {
                // Facade Pattern — cukup panggil service
                _daftarObat = await _obatService.GetAllAsync();
                TampilkanData(_daftarObat);

                int tersedia = _daftarObat.Count(o => o.GetProperty("stok").GetInt32() > 0);
                int habis = _daftarObat.Count - tersedia;

                lblStatus.ForeColor = Color.FromArgb(100, 116, 139);
                lblStatus.Text = $"Total Obat: {_daftarObat.Count} | Tersedia: {tersedia} | Habis: {habis}";
            }
            catch
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Tidak dapat terhubung ke server.";
            }
        }

        private void TampilkanData(List<JsonElement> data)
        {
            dgvObat.Rows.Clear();
            foreach (var o in data)
            {
                int harga = o.GetProperty("harga").GetInt32();
                dgvObat.Rows.Add(
                    o.GetProperty("id"),
                    o.GetProperty("namaObat").GetString(),
                    o.GetProperty("stok"),
                    o.GetProperty("statusObat").GetString(),
                    $"Rp{harga:N0}"
                );
            }
        }

        private void TxtCari_TextChanged(object? sender, EventArgs e)
        {
            string keyword = txtCari.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(keyword)) { TampilkanData(_daftarObat); return; }

            var hasil = _daftarObat
                .Where(o => o.GetProperty("namaObat").GetString()!.ToLower().Contains(keyword))
                .ToList();
            TampilkanData(hasil);
        }

        private void DgvObat_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvObat.Columns[e.ColumnIndex].Name != "Status") return;
            string status = dgvObat.Rows[e.RowIndex].Cells["Status"].Value?.ToString() ?? "";
            e.CellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            e.CellStyle.ForeColor = status.Equals("Tersedia", StringComparison.OrdinalIgnoreCase)
                ? Color.FromArgb(22, 163, 74)
                : Color.FromArgb(220, 38, 38);
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // FormPesanan — dengan hitung total otomatis & validasi stok
    // ══════════════════════════════════════════════════════════════════════
    public class FormPesanan : Form
    {
        private Label lblJudul;
        private DataGridView dgvObat;
        private TextBox txtJumlah;
        private ComboBox cmbMetode;
        private TextBox txtPembayaran;
        private Label lblTotal;
        private Label lblStatus;
        private Button btnPesan;
        private Button btnRefresh;

        // Facade Pattern — pakai service bukan HttpClient langsung
        private readonly ObatService _obatService;
        private readonly PesananService _pesananService;
        private readonly int _pelangganId;

        private List<JsonElement> _daftarObat = new();
        private int? selectedObatId = null;
        private int selectedStok = 0;
        private int selectedHarga = 0;

        public FormPesanan(ObatService obatService, PesananService pesananService, int pelangganId)
        {
            _obatService = obatService;
            _pesananService = pesananService;
            _pelangganId = pelangganId;
            InitializeComponent();
            _ = LoadObatAsync();
        }

        private void InitializeComponent()
        {
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9f);

            lblJudul = new Label
            {
                Text = "Pesan Obat",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 20),
                AutoSize = true
            };

            dgvObat = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(640, 260),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvObat.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvObat.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvObat.EnableHeadersVisualStyles = false;
            dgvObat.Columns.Add("Id", "ID");
            dgvObat.Columns.Add("NamaObat", "Nama Obat");
            dgvObat.Columns.Add("Stok", "Stok");
            dgvObat.Columns.Add("Status", "Status");
            dgvObat.Columns.Add("Harga", "Harga");
            dgvObat.Columns["Id"].FillWeight = 30;
            dgvObat.CellClick += DgvObat_CellClick;

            var pnlForm = new Panel
            {
                Location = new Point(20, 335),
                Size = new Size(640, 150),
                BackColor = Color.White
            };
            pnlForm.Paint += (s, e) =>
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(226, 232, 240)),
                    0, 0, pnlForm.Width - 1, pnlForm.Height - 1);

            pnlForm.Controls.Add(BuatLabel("Jumlah", new Point(15, 15)));
            pnlForm.Controls.Add(BuatLabel("Metode Pengambilan", new Point(120, 15)));
            pnlForm.Controls.Add(BuatLabel("Pembayaran (Rp)", new Point(320, 15)));

            txtJumlah = BuatTextBox(new Point(15, 35), 90);
            txtJumlah.TextChanged += HitungTotal;
            pnlForm.Controls.Add(txtJumlah);

            cmbMetode = new ComboBox
            {
                Location = new Point(120, 35),
                Size = new Size(180, 26),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9f)
            };
            cmbMetode.Items.AddRange(new object[] { "Langsung", "Diantar" });
            cmbMetode.SelectedIndex = 0;
            pnlForm.Controls.Add(cmbMetode);

            txtPembayaran = BuatTextBox(new Point(320, 35), 150);
            pnlForm.Controls.Add(txtPembayaran);

            lblTotal = new Label
            {
                Text = "Total: Rp0",
                Location = new Point(490, 38),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 163, 74)
            };
            pnlForm.Controls.Add(lblTotal);

            btnPesan = new Button
            {
                Text = "Buat Pesanan",
                Location = new Point(15, 90),
                Size = new Size(140, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPesan.FlatAppearance.BorderSize = 0;
            btnPesan.Click += BtnPesan_Click;
            pnlForm.Controls.Add(btnPesan);

            btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(170, 90),
                Size = new Size(100, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 116, 139),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += async (s, e) => { ResetForm(); await LoadObatAsync(); };
            pnlForm.Controls.Add(btnRefresh);

            lblStatus = new Label
            {
                Text = "Pilih obat dari tabel.",
                Location = new Point(20, 500),
                AutoSize = true,
                ForeColor = Color.FromArgb(100, 116, 139),
                Font = new Font("Segoe UI", 8.5f)
            };

            Controls.AddRange(new Control[] { lblJudul, dgvObat, pnlForm, lblStatus });
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

        private void DgvObat_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvObat.Rows[e.RowIndex];
            selectedObatId = Convert.ToInt32(row.Cells["Id"].Value?.ToString());
            selectedStok = Convert.ToInt32(row.Cells["Stok"].Value);
            string hargaText = row.Cells["Harga"].Value?.ToString()?.Replace("Rp", "").Replace(".", "").Replace(",", "") ?? "0";
            int.TryParse(hargaText, out selectedHarga);
            HitungTotal(null, EventArgs.Empty);
            lblStatus.ForeColor = Color.FromArgb(37, 99, 235);
            lblStatus.Text = $"Obat dipilih | Stok: {selectedStok} | Harga: Rp{selectedHarga:N0}";
        }

        private void HitungTotal(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtJumlah.Text, out int jumlah)) { lblTotal.Text = "Total: Rp0"; return; }
            lblTotal.Text = $"Total: Rp{(jumlah * selectedHarga):N0}";
        }

        private async Task LoadObatAsync()
        {
            try
            {
                // Facade Pattern — cukup panggil service
                _daftarObat = await _obatService.GetAllAsync();
                dgvObat.Rows.Clear();
                foreach (var o in _daftarObat)
                {
                    int stok = o.GetProperty("stok").GetInt32();
                    int rowIndex = dgvObat.Rows.Add(
                        o.GetProperty("id"),
                        o.GetProperty("namaObat").GetString(),
                        stok,
                        o.GetProperty("statusObat").GetString(),
                        $"Rp{o.GetProperty("harga"):N0}"
                    );
                    if (stok <= 0)
                        dgvObat.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Red;
                }
                lblStatus.ForeColor = Color.FromArgb(100, 116, 139);
                lblStatus.Text = $"{_daftarObat.Count} obat tersedia";
            }
            catch
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Tidak dapat terhubung ke server.";
            }
        }

        private async void BtnPesan_Click(object? sender, EventArgs e)
        {
            if (selectedObatId == null) { TampilError("Pilih obat terlebih dahulu."); return; }
            if (!int.TryParse(txtJumlah.Text, out int jumlah) || jumlah <= 0) { TampilError("Jumlah harus lebih dari 0."); return; }
            if (selectedStok <= 0) { TampilError("Obat sedang tidak tersedia."); return; }
            if (jumlah > selectedStok) { TampilError("Jumlah melebihi stok tersedia."); return; }
            if (!int.TryParse(txtPembayaran.Text, out int pembayaran) || pembayaran <= 0) { TampilError("Pembayaran tidak valid."); return; }
            int totalHarga = jumlah * selectedHarga;
            if (pembayaran < totalHarga) { TampilError($"Pembayaran kurang. Minimal Rp{totalHarga:N0}"); return; }

            btnPesan.Enabled = false;
            try
            {
                string metode = cmbMetode.SelectedItem?.ToString() ?? "Langsung";
                var (sukses, pesan) = await _pesananService.BuatPesananAsync(
                    _pelangganId, selectedObatId.Value, jumlah, metode, pembayaran);

                if (sukses)
                {
                    lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
                    lblStatus.Text = "Pesanan berhasil dibuat.";
                    ResetForm();
                    await LoadObatAsync();
                }
                else
                {
                    TampilError($"Gagal: {pesan}");
                }
            }
            catch
            {
                TampilError("Tidak dapat terhubung ke server.");
            }
            finally { btnPesan.Enabled = true; }
        }

        private void TampilError(string pesan)
        {
            lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
            lblStatus.Text = pesan;
        }

        private void ResetForm()
        {
            txtJumlah.Clear(); txtPembayaran.Clear();
            cmbMetode.SelectedIndex = 0;
            selectedObatId = null; selectedStok = 0; selectedHarga = 0;
            dgvObat.ClearSelection();
            lblTotal.Text = "Total: Rp0";
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // FormRiwayat — dengan fitur search & color formatting status
    // ══════════════════════════════════════════════════════════════════════
    public class FormRiwayat : Form
    {
        private Label lblJudul;
        private Label lblStatus;
        private TextBox txtCari;
        private Button btnRefresh;
        private DataGridView dgvRiwayat;

        // Facade Pattern — pakai PesananService bukan HttpClient langsung
        private readonly PesananService _pesananService;
        private readonly int _pelangganId;
        private List<JsonElement> _daftarRiwayat = new();

        public FormRiwayat(PesananService pesananService, int pelangganId)
        {
            _pesananService = pesananService;
            _pelangganId = pelangganId;
            InitializeComponent();
            _ = LoadAsync();
        }

        private void InitializeComponent()
        {
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9f);

            lblJudul = new Label
            {
                Text = "Riwayat Pembelian",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 20),
                AutoSize = true
            };

            txtCari = new TextBox
            {
                Location = new Point(20, 60),
                Size = new Size(250, 28),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9f),
                PlaceholderText = "Cari ID Pesanan / Nama Obat..."
            };
            txtCari.TextChanged += TxtCari_TextChanged;

            btnRefresh = new Button
            {
                Text = "↻ Refresh",
                Location = new Point(560, 58),
                Size = new Size(110, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 116, 139),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += async (s, e) => { txtCari.Clear(); await LoadAsync(); };

            dgvRiwayat = new DataGridView
            {
                Location = new Point(20, 105),
                Size = new Size(650, 360),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9f),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvRiwayat.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvRiwayat.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(71, 85, 105);
            dgvRiwayat.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvRiwayat.EnableHeadersVisualStyles = false;

            dgvRiwayat.Columns.Add("IdPesanan", "ID Pesanan");
            dgvRiwayat.Columns.Add("Obat", "Obat");
            dgvRiwayat.Columns.Add("Jumlah", "Jumlah");
            dgvRiwayat.Columns.Add("Metode", "Metode");
            dgvRiwayat.Columns.Add("Pembayaran", "Pembayaran");
            dgvRiwayat.Columns.Add("Status", "Status");
            dgvRiwayat.Columns["Jumlah"].FillWeight = 50;
            dgvRiwayat.Columns["Metode"].FillWeight = 90;
            dgvRiwayat.Columns["Status"].FillWeight = 90;
            dgvRiwayat.CellFormatting += DgvRiwayat_CellFormatting;

            lblStatus = new Label
            {
                Text = "Memuat data...",
                Location = new Point(20, 480),
                Size = new Size(650, 40),
                ForeColor = Color.FromArgb(100, 116, 139),
                Font = new Font("Segoe UI", 8.5f)
            };

            Controls.AddRange(new Control[] { lblJudul, txtCari, btnRefresh, dgvRiwayat, lblStatus });
        }

        private async Task LoadAsync()
        {
            try
            {
                // Facade Pattern — cukup panggil service
                _daftarRiwayat = await _pesananService.GetByPelangganAsync(_pelangganId);
                TampilkanData(_daftarRiwayat);

                int totalPembayaran = _daftarRiwayat.Sum(p => p.GetProperty("pembayaran").GetInt32());
                lblStatus.ForeColor = Color.FromArgb(100, 116, 139);
                lblStatus.Text = $"Total Transaksi: {_daftarRiwayat.Count} | Total Pembelian: Rp{totalPembayaran:N0}";
            }
            catch
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Tidak dapat terhubung ke server.";
            }
        }

        private void TampilkanData(List<JsonElement> data)
        {
            dgvRiwayat.Rows.Clear();
            foreach (var p in data)
            {
                string namaObat = "-";
                if (p.TryGetProperty("obat", out var obat) && obat.ValueKind != JsonValueKind.Null)
                    namaObat = obat.GetProperty("namaObat").GetString() ?? "-";

                int pembayaran = p.GetProperty("pembayaran").GetInt32();
                dgvRiwayat.Rows.Add(
                    p.GetProperty("idPesanan").GetString(),
                    namaObat,
                    p.GetProperty("jumlah"),
                    p.GetProperty("metodePengambilan").GetString(),
                    $"Rp{pembayaran:N0}",
                    p.GetProperty("status").GetString()
                );
            }
        }

        private void TxtCari_TextChanged(object? sender, EventArgs e)
        {
            string keyword = txtCari.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(keyword)) { TampilkanData(_daftarRiwayat); return; }

            var hasil = _daftarRiwayat.Where(p =>
            {
                string idPesanan = p.GetProperty("idPesanan").GetString()?.ToLower() ?? "";
                string namaObat = "";
                if (p.TryGetProperty("obat", out var obat) && obat.ValueKind != JsonValueKind.Null)
                    namaObat = obat.GetProperty("namaObat").GetString()?.ToLower() ?? "";
                return idPesanan.Contains(keyword) || namaObat.Contains(keyword);
            }).ToList();

            TampilkanData(hasil);
        }

        private void DgvRiwayat_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvRiwayat.Columns[e.ColumnIndex].Name != "Status") return;
            string status = dgvRiwayat.Rows[e.RowIndex].Cells["Status"].Value?.ToString() ?? "";
            e.CellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            e.CellStyle.ForeColor = status.ToLower() switch
            {
                "keranjang" => Color.FromArgb(234, 179, 8),
                "diproses" => Color.FromArgb(37, 99, 235),
                "disiapkan" => Color.FromArgb(22, 163, 74),
                "selesai" => Color.FromArgb(21, 128, 61),
                "dibatalkan" => Color.FromArgb(220, 38, 38),
                _ => Color.FromArgb(100, 116, 139)
            };
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // FormGantiPassword — dengan tombol Reset
    // ══════════════════════════════════════════════════════════════════════
    public class FormGantiPassword : Form
    {
        private TextBox txtPasswordBaru;
        private TextBox txtKonfirm;
        private Button btnSimpan;
        private Button btnReset;
        private Label lblStatus;

        // Facade Pattern — pakai PelangganService bukan HttpClient langsung
        private readonly PelangganService _pelangganService;
        private readonly int _pelangganId;
        private readonly string _noTelp;

        public FormGantiPassword(PelangganService pelangganService, int pelangganId, string noTelp)
        {
            _pelangganService = pelangganService;
            _pelangganId = pelangganId;
            _noTelp = noTelp;
            InitializeComponent();
            ResetForm();
        }

        private void InitializeComponent()
        {
            Text = "Ganti Password";
            Size = new Size(380, 300);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9f);

            Controls.Add(new Label
            {
                Text = "Ganti Password",
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 20),
                AutoSize = true
            });

            Controls.Add(new Label { Text = "Password Baru", Location = new Point(20, 65), AutoSize = true, Font = new Font("Segoe UI", 8.5f, FontStyle.Bold) });
            txtPasswordBaru = new TextBox { Location = new Point(20, 85), Size = new Size(320, 26), PasswordChar = '●', BorderStyle = BorderStyle.FixedSingle };
            Controls.Add(txtPasswordBaru);

            Controls.Add(new Label { Text = "Konfirmasi Password", Location = new Point(20, 120), AutoSize = true, Font = new Font("Segoe UI", 8.5f, FontStyle.Bold) });
            txtKonfirm = new TextBox { Location = new Point(20, 140), Size = new Size(320, 26), PasswordChar = '●', BorderStyle = BorderStyle.FixedSingle };
            Controls.Add(txtKonfirm);

            lblStatus = new Label { Text = "", Location = new Point(20, 175), Size = new Size(320, 20), ForeColor = Color.FromArgb(100, 116, 139) };
            Controls.Add(lblStatus);

            btnSimpan = new Button
            {
                Text = "Simpan",
                Location = new Point(20, 210),
                Size = new Size(150, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnSimpan.FlatAppearance.BorderSize = 0;
            btnSimpan.Click += BtnSimpan_Click;
            Controls.Add(btnSimpan);

            btnReset = new Button
            {
                Text = "Reset",
                Location = new Point(190, 210),
                Size = new Size(150, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(226, 232, 240),
                ForeColor = Color.FromArgb(51, 65, 85),
                Cursor = Cursors.Hand
            };
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.Click += (s, e) => ResetForm();
            Controls.Add(btnReset);
        }

        private void ResetForm()
        {
            txtPasswordBaru.Text = "";
            txtKonfirm.Text = "";
            lblStatus.Text = "";
            lblStatus.ForeColor = Color.FromArgb(100, 116, 139);
        }

        private async void BtnSimpan_Click(object? sender, EventArgs e)
        {
            lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
            if (string.IsNullOrWhiteSpace(txtPasswordBaru.Text)) { lblStatus.Text = "Password tidak boleh kosong."; return; }
            if (txtPasswordBaru.Text.Length < 6) { lblStatus.Text = "Password minimal 6 karakter."; return; }
            if (!txtPasswordBaru.Text.Any(char.IsUpper)) { lblStatus.Text = "Password harus mengandung 1 huruf kapital."; return; }
            if (!txtPasswordBaru.Text.Any(char.IsDigit)) { lblStatus.Text = "Password harus mengandung minimal 1 angka."; return; }
            if (txtPasswordBaru.Text != txtKonfirm.Text) { lblStatus.Text = "Password tidak cocok."; return; }

            try
            {
                // Facade Pattern — cukup panggil service
                var (sukses, _) = await _pelangganService.GantiPasswordAsync(
                    _pelangganId, _noTelp, txtPasswordBaru.Text);

                if (sukses)
                {
                    MessageBox.Show("Password berhasil diubah.", "Berhasil",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetForm();
                    Close();
                }
                else
                {
                    lblStatus.Text = "Gagal mengubah password.";
                }
            }
            catch
            {
                lblStatus.Text = "Tidak dapat terhubung ke server.";
            }
        }
    }
}