using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace PROJECTKPL.GUI
{
    public class FormDaftarObat : Form
    {
        private Label lblJudul;
        private Label lblStatus;
        private TextBox txtCari;
        private Button btnRefresh;
        private DataGridView dgvObat;

        private readonly HttpClient _http;
        private List<JsonElement> _daftarObat = new();

        public FormDaftarObat(HttpClient http)
        {
            _http = http;
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

            dgvObat.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(248, 250, 252);

            dgvObat.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.FromArgb(71, 85, 105);

            dgvObat.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9f, FontStyle.Bold);

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

            Controls.AddRange(new Control[]
            {
            lblJudul,
            txtCari,
            btnRefresh,
            dgvObat,
            lblStatus
            });
        }

        private async Task LoadAsync()
        {
            try
            {
                var res = await _http.GetAsync("api/obat");

                if (!res.IsSuccessStatusCode)
                {
                    lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                    lblStatus.Text = "Gagal mengambil data obat.";
                    return;
                }

                _daftarObat =
                    await res.Content.ReadFromJsonAsync<List<JsonElement>>()
                    ?? new();

                TampilkanData(_daftarObat);

                int tersedia = _daftarObat.Count(o =>
                    o.GetProperty("stok").GetInt32() > 0);

                int habis = _daftarObat.Count - tersedia;

                lblStatus.ForeColor = Color.FromArgb(100, 116, 139);
                lblStatus.Text =
                    $"Total Obat: {_daftarObat.Count} | " +
                    $"Tersedia: {tersedia} | " +
                    $"Habis: {habis}";
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

            if (string.IsNullOrEmpty(keyword))
            {
                TampilkanData(_daftarObat);
                return;
            }

            var hasil = _daftarObat
                .Where(o =>
                    o.GetProperty("namaObat")
                     .GetString()!
                     .ToLower()
                     .Contains(keyword))
                .ToList();

            TampilkanData(hasil);
        }

        private void DgvObat_CellFormatting(
            object? sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (dgvObat.Columns[e.ColumnIndex].Name != "Status")
                return;

            string status =
                dgvObat.Rows[e.RowIndex]
                .Cells["Status"]
                .Value?.ToString() ?? "";

            if (status.Equals("Tersedia",
                StringComparison.OrdinalIgnoreCase))
            {
                e.CellStyle.ForeColor =
                    Color.FromArgb(22, 163, 74);
                e.CellStyle.Font =
                    new Font("Segoe UI", 9f, FontStyle.Bold);
            }
            else
            {
                e.CellStyle.ForeColor =
                    Color.FromArgb(220, 38, 38);
                e.CellStyle.Font =
                    new Font("Segoe UI", 9f, FontStyle.Bold);
            }
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // FormPesanan — pelanggan buat pesanan
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

        private readonly HttpClient _http;
        private readonly int _pelangganId;

        private List<JsonElement> _daftarObat = new();

        private int? selectedObatId = null;
        private int selectedStok = 0;
        private int selectedHarga = 0;

        public FormPesanan(HttpClient http, int pelangganId)
        {
            _http = http;
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

            dgvObat.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(248, 250, 252);

            dgvObat.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9f, FontStyle.Bold);

            dgvObat.EnableHeadersVisualStyles = false;

            dgvObat.Columns.Add("Id", "ID");
            dgvObat.Columns.Add("NamaObat", "Nama Obat");
            dgvObat.Columns.Add("Stok", "Stok");
            dgvObat.Columns.Add("Status", "Status");
            dgvObat.Columns.Add("Harga", "Harga");

            dgvObat.Columns["Id"].FillWeight = 30;

            dgvObat.CellClick += DgvObat_CellClick;

            // =====================================================
            // PANEL FORM
            // =====================================================

            Panel pnlForm = new Panel
            {
                Location = new Point(20, 335),
                Size = new Size(640, 150),
                BackColor = Color.White
            };

            pnlForm.Paint += (s, e) =>
            {
                e.Graphics.DrawRectangle(
                    new Pen(Color.FromArgb(226, 232, 240)),
                    0,
                    0,
                    pnlForm.Width - 1,
                    pnlForm.Height - 1);
            };

            var lblJml = BuatLabel("Jumlah", new Point(15, 15));

            var lblMtd = BuatLabel(
                "Metode Pengambilan",
                new Point(120, 15));

            var lblByr = BuatLabel(
                "Pembayaran (Rp)",
                new Point(320, 15));

            txtJumlah = BuatTextBox(
                new Point(15, 35),
                90);

            txtJumlah.TextChanged += HitungTotal;

            cmbMetode = new ComboBox
            {
                Location = new Point(120, 35),
                Size = new Size(180, 26),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9f)
            };

            cmbMetode.Items.AddRange(
                new object[]
                {
                "Langsung",
                "Diantar"
                });

            cmbMetode.SelectedIndex = 0;

            txtPembayaran = BuatTextBox(
                new Point(320, 35),
                150);

            lblTotal = new Label
            {
                Text = "Total: Rp0",
                Location = new Point(490, 38),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 163, 74)
            };

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

            btnRefresh.Click += async (s, e) =>
            {
                ResetForm();
                await LoadObatAsync();
            };

            pnlForm.Controls.AddRange(new Control[]
            {
            lblJml,
            lblMtd,
            lblByr,

            txtJumlah,
            cmbMetode,
            txtPembayaran,

            lblTotal,

            btnPesan,
            btnRefresh
            });

            lblStatus = new Label
            {
                Text = "Pilih obat dari tabel.",
                Location = new Point(20, 500),
                AutoSize = true,
                ForeColor = Color.FromArgb(100, 116, 139),
                Font = new Font("Segoe UI", 8.5f)
            };

            Controls.AddRange(new Control[]
            {
            lblJudul,
            dgvObat,
            pnlForm,
            lblStatus
            });
        }

        private Label BuatLabel(string text, Point loc)
        {
            return new Label
            {
                Text = text,
                Location = loc,
                AutoSize = true,
                ForeColor = Color.FromArgb(71, 85, 105),
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
            };
        }

        private TextBox BuatTextBox(Point loc, int width)
        {
            return new TextBox
            {
                Location = loc,
                Size = new Size(width, 26),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9f)
            };
        }

        private void DgvObat_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var row = dgvObat.Rows[e.RowIndex];

            selectedObatId =
                Convert.ToInt32(row.Cells["Id"].Value?.ToString());

            selectedStok =
                Convert.ToInt32(row.Cells["Stok"].Value);

            string hargaText =
                row.Cells["Harga"].Value?.ToString()?
                .Replace("Rp", "")
                .Replace(".", "")
                .Replace(",", "")
                ?? "0";

            int.TryParse(hargaText, out selectedHarga);

            HitungTotal(null, EventArgs.Empty);

            lblStatus.ForeColor =
                Color.FromArgb(37, 99, 235);

            lblStatus.Text =
                $"Obat dipilih | Stok: {selectedStok} | Harga: Rp{selectedHarga:N0}";
        }

        private void HitungTotal(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtJumlah.Text, out int jumlah))
            {
                lblTotal.Text = "Total: Rp0";
                return;
            }

            int total = jumlah * selectedHarga;

            lblTotal.Text =
                $"Total: Rp{total:N0}";
        }

        private async Task LoadObatAsync()
        {
            try
            {
                var res = await _http.GetAsync("api/obat");

                if (!res.IsSuccessStatusCode)
                {
                    lblStatus.Text = "Gagal memuat data obat.";
                    return;
                }

                _daftarObat =
                    await res.Content.ReadFromJsonAsync<List<JsonElement>>()
                    ?? new();

                dgvObat.Rows.Clear();

                foreach (var o in _daftarObat)
                {
                    int stok =
                        o.GetProperty("stok").GetInt32();

                    int rowIndex =
                        dgvObat.Rows.Add(
                            o.GetProperty("id"),
                            o.GetProperty("namaObat").GetString(),
                            stok,
                            o.GetProperty("statusObat").GetString(),
                            $"Rp{o.GetProperty("harga"):N0}"
                        );

                    if (stok <= 0)
                    {
                        dgvObat.Rows[rowIndex]
                            .DefaultCellStyle.ForeColor = Color.Red;
                    }
                }

                lblStatus.ForeColor =
                    Color.FromArgb(100, 116, 139);

                lblStatus.Text =
                    $"{_daftarObat.Count} obat tersedia";
            }
            catch
            {
                lblStatus.ForeColor =
                    Color.FromArgb(220, 38, 38);

                lblStatus.Text =
                    "Tidak dapat terhubung ke server.";
            }
        }

        private async void BtnPesan_Click(object? sender, EventArgs e)
        {
            if (selectedObatId == null)
            {
                lblStatus.ForeColor =
                    Color.FromArgb(220, 38, 38);

                lblStatus.Text =
                    "Pilih obat terlebih dahulu.";

                return;
            }

            if (!int.TryParse(txtJumlah.Text, out int jumlah) ||
                jumlah <= 0)
            {
                lblStatus.ForeColor =
                    Color.FromArgb(220, 38, 38);

                lblStatus.Text =
                    "Jumlah harus lebih dari 0.";

                return;
            }

            if (selectedStok <= 0)
            {
                lblStatus.ForeColor =
                    Color.FromArgb(220, 38, 38);

                lblStatus.Text =
                    "Obat sedang tidak tersedia.";

                return;
            }

            if (jumlah > selectedStok)
            {
                lblStatus.ForeColor =
                    Color.FromArgb(220, 38, 38);

                lblStatus.Text =
                    "Jumlah melebihi stok tersedia.";

                return;
            }

            if (!int.TryParse(txtPembayaran.Text, out int pembayaran) ||
                pembayaran <= 0)
            {
                lblStatus.ForeColor =
                    Color.FromArgb(220, 38, 38);

                lblStatus.Text =
                    "Pembayaran tidak valid.";

                return;
            }

            int totalHarga =
                jumlah * selectedHarga;

            if (pembayaran < totalHarga)
            {
                lblStatus.ForeColor =
                    Color.FromArgb(220, 38, 38);

                lblStatus.Text =
                    $"Pembayaran kurang. Minimal Rp{totalHarga:N0}";

                return;
            }

            btnPesan.Enabled = false;

            try
            {
                string metode =
                    cmbMetode.SelectedItem?.ToString()
                    ?? "Langsung";

                var res =
                    await _http.PostAsJsonAsync(
                        "api/pesanan",
                        new
                        {
                            pelangganId = _pelangganId,
                            obatId = selectedObatId,
                            jumlah,
                            metodePengambilan = metode,
                            pembayaran
                        });

                if (res.IsSuccessStatusCode)
                {
                    lblStatus.ForeColor =
                        Color.FromArgb(22, 163, 74);

                    lblStatus.Text =
                        "Pesanan berhasil dibuat.";

                    ResetForm();

                    await LoadObatAsync();
                }
                else
                {
                    string msg =
                        await res.Content.ReadAsStringAsync();

                    lblStatus.ForeColor =
                        Color.FromArgb(220, 38, 38);

                    lblStatus.Text =
                        $"Gagal: {msg}";
                }
            }
            catch
            {
                lblStatus.ForeColor =
                    Color.FromArgb(220, 38, 38);

                lblStatus.Text =
                    "Tidak dapat terhubung ke server.";
            }
            finally
            {
                btnPesan.Enabled = true;
            }
        }

        private void ResetForm()
        {
            txtJumlah.Clear();
            txtPembayaran.Clear();

            cmbMetode.SelectedIndex = 0;

            selectedObatId = null;
            selectedStok = 0;
            selectedHarga = 0;

            dgvObat.ClearSelection();

            lblTotal.Text = "Total: Rp0";
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // FormRiwayat — riwayat pembelian pelanggan
    // ══════════════════════════════════════════════════════════════════════
    public class FormRiwayat : Form
    {
        private Label lblJudul;
        private Label lblStatus;
        private TextBox txtCari;
        private Button btnRefresh;
        private DataGridView dgvRiwayat;

        private readonly HttpClient _http;
        private readonly int _pelangganId;

        private List<JsonElement> _daftarRiwayat = new();

        public FormRiwayat(HttpClient http, int pelangganId)
        {
            _http = http;
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
            btnRefresh.Click += async (s, e) =>
            {
                txtCari.Clear();
                await LoadAsync();
            };

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

            dgvRiwayat.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(248, 250, 252);

            dgvRiwayat.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.FromArgb(71, 85, 105);

            dgvRiwayat.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9f, FontStyle.Bold);

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

            Controls.AddRange(new Control[]
            {
            lblJudul,
            txtCari,
            btnRefresh,
            dgvRiwayat,
            lblStatus
            });
        }

        private async Task LoadAsync()
        {
            try
            {
                var res =
                    await _http.GetAsync(
                        $"api/pesanan/pelanggan/{_pelangganId}");

                if (!res.IsSuccessStatusCode)
                {
                    lblStatus.ForeColor =
                        Color.FromArgb(220, 38, 38);

                    lblStatus.Text =
                        "Gagal mengambil data riwayat.";

                    return;
                }

                _daftarRiwayat =
                    await res.Content.ReadFromJsonAsync<List<JsonElement>>()
                    ?? new();

                TampilkanData(_daftarRiwayat);

                int totalPembayaran = 0;

                foreach (var p in _daftarRiwayat)
                {
                    totalPembayaran +=
                        p.GetProperty("pembayaran").GetInt32();
                }

                lblStatus.ForeColor =
                    Color.FromArgb(100, 116, 139);

                lblStatus.Text =
                    $"Total Transaksi: {_daftarRiwayat.Count} | " +
                    $"Total Pembelian: Rp{totalPembayaran:N0}";
            }
            catch
            {
                lblStatus.ForeColor =
                    Color.FromArgb(220, 38, 38);

                lblStatus.Text =
                    "Tidak dapat terhubung ke server.";
            }
        }

        private void TampilkanData(List<JsonElement> data)
        {
            dgvRiwayat.Rows.Clear();

            foreach (var p in data)
            {
                string namaObat = "-";

                if (p.TryGetProperty("obat", out var obat)
                    && obat.ValueKind != JsonValueKind.Null)
                {
                    namaObat =
                        obat.GetProperty("namaObat")
                            .GetString() ?? "-";
                }

                int pembayaran =
                    p.GetProperty("pembayaran").GetInt32();

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
            string keyword =
                txtCari.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                TampilkanData(_daftarRiwayat);
                return;
            }

            var hasil =
                _daftarRiwayat
                .Where(p =>
                {
                    string idPesanan =
                        p.GetProperty("idPesanan")
                         .GetString()?
                         .ToLower() ?? "";

                    string namaObat = "";

                    if (p.TryGetProperty("obat", out var obat)
                        && obat.ValueKind != JsonValueKind.Null)
                    {
                        namaObat =
                            obat.GetProperty("namaObat")
                                .GetString()?
                                .ToLower() ?? "";
                    }

                    return idPesanan.Contains(keyword)
                           || namaObat.Contains(keyword);
                })
                .ToList();

            TampilkanData(hasil);
        }

        private void DgvRiwayat_CellFormatting(
            object? sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (dgvRiwayat.Columns[e.ColumnIndex].Name != "Status")
                return;

            string status =
                dgvRiwayat.Rows[e.RowIndex]
                .Cells["Status"]
                .Value?.ToString() ?? "";

            e.CellStyle.Font =
                new Font("Segoe UI", 9f, FontStyle.Bold);

            switch (status.ToLower())
            {
                case "menunggu":
                    e.CellStyle.ForeColor =
                        Color.FromArgb(234, 179, 8);
                    break;

                case "diproses":
                    e.CellStyle.ForeColor =
                        Color.FromArgb(37, 99, 235);
                    break;

                case "siapdiambil":
                case "siap diambil":
                    e.CellStyle.ForeColor =
                        Color.FromArgb(22, 163, 74);
                    break;

                case "selesai":
                    e.CellStyle.ForeColor =
                        Color.FromArgb(21, 128, 61);
                    break;

                case "dibatalkan":
                    e.CellStyle.ForeColor =
                        Color.FromArgb(220, 38, 38);
                    break;
            }
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // FormGantiPassword — ganti password pelanggan
    // ══════════════════════════════════════════════════════════════════════
    public class FormGantiPassword : Form
    {
        private TextBox txtPasswordBaru;
        private TextBox txtKonfirm;
        private Button btnSimpan;
        private Button btnReset;
        private Label lblStatus;

        private readonly HttpClient _http;
        private readonly int _pelangganId;
        private readonly string _noTelp;

        public FormGantiPassword(HttpClient http, int pelangganId, string noTelp)
        {
            _http = http;
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

            var lblJudul = new Label
            {
                Text = "Ganti Password",
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 20),
                AutoSize = true
            };

            var lblBaru = new Label
            {
                Text = "Password Baru",
                Location = new Point(20, 65),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
            };

            txtPasswordBaru = new TextBox
            {
                Location = new Point(20, 85),
                Size = new Size(320, 26),
                PasswordChar = '●',
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblKfm = new Label
            {
                Text = "Konfirmasi Password",
                Location = new Point(20, 120),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
            };

            txtKonfirm = new TextBox
            {
                Location = new Point(20, 140),
                Size = new Size(320, 26),
                PasswordChar = '●',
                BorderStyle = BorderStyle.FixedSingle
            };

            lblStatus = new Label
            {
                Text = "",
                Location = new Point(20, 175),
                Size = new Size(320, 20),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

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

            Controls.AddRange(new Control[]
            {
            lblJudul,
            lblBaru, txtPasswordBaru,
            lblKfm, txtKonfirm,
            lblStatus,
            btnSimpan, btnReset
            });
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

            if (string.IsNullOrWhiteSpace(txtPasswordBaru.Text))
            {
                lblStatus.Text = "Password tidak boleh kosong.";
                return;
            }

            if (txtPasswordBaru.Text.Length < 6)
            {
                lblStatus.Text = "Password minimal 6 karakter.";
                return;
            }

            if (txtPasswordBaru.Text != txtKonfirm.Text)
            {
                lblStatus.Text = "Password tidak cocok.";
                return;
            }

            try
            {
                var res = await _http.PutAsJsonAsync(
                    $"api/pelanggan/{_pelangganId}/password",
                    new
                    {
                        noTelp = _noTelp,
                        passwordBaru = txtPasswordBaru.Text
                    });

                if (res.IsSuccessStatusCode)
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
