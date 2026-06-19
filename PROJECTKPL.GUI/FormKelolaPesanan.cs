using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace PROJECTKPL.GUI
{
    public class FormKelolaPesanan : Form
    {
        private Label lblJudul;
        private DataGridView dgvPesanan;
        private Panel pnlAksi;
        private Button btnKonfirmasi;
        private Button btnSiapkan;
        private Button btnSelesai;
        private Button btnBatalkan;
        private Button btnRefresh;
        private Label lblStatus;

        private readonly HttpClient _http;

        public FormKelolaPesanan(HttpClient http)
        {
            _http = http;
            InitializeComponent();
            _ = LoadPesananAsync();
        }

        private void InitializeComponent()
        {
            Text = "Kelola Pesanan";
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9f);

            lblJudul = new Label
            {
                Text = "Kelola Pesanan",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 20),
                AutoSize = true
            };

            dgvPesanan = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(660, 360),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9f),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvPesanan.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvPesanan.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(71, 85, 105);
            dgvPesanan.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            dgvPesanan.EnableHeadersVisualStyles = false;
            dgvPesanan.Columns.Add("Id", "ID");
            dgvPesanan.Columns.Add("IdPesanan", "ID Pesanan");
            dgvPesanan.Columns.Add("Pelanggan", "Pelanggan");
            dgvPesanan.Columns.Add("Obat", "Obat");
            dgvPesanan.Columns.Add("Jumlah", "Jml");
            dgvPesanan.Columns.Add("Metode", "Metode");
            dgvPesanan.Columns.Add("Pembayaran", "Pembayaran");
            dgvPesanan.Columns.Add("Status", "Status");
            dgvPesanan.Columns["Id"].FillWeight = 30;
            dgvPesanan.Columns["Jumlah"].FillWeight = 30;
            dgvPesanan.Columns["Metode"].FillWeight = 60;

            // ── Panel aksi state machine ───────────────────────────────────
            pnlAksi = new Panel
            {
                Location = new Point(20, 432),
                Size = new Size(660, 70),
                BackColor = Color.White
            };
            pnlAksi.Paint += (s, e) =>
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(226, 232, 240)), 0, 0, pnlAksi.Width - 1, pnlAksi.Height - 1);

            var lblAksi = new Label
            {
                Text = "Aksi State Machine — pilih baris pesanan dulu:",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                Location = new Point(15, 10),
                AutoSize = true
            };

            btnKonfirmasi = BuatTombol("✓ Konfirmasi", new Point(15, 32), Color.FromArgb(37, 99, 235));
            btnKonfirmasi.Click += async (s, e) => await AktifkanTrigger("Konfirmasi");

            btnSiapkan = BuatTombol("⚙️ Siapkan", new Point(120, 32), Color.FromArgb(234, 179, 8));
            btnSiapkan.Click += async (s, e) => await AktifkanTrigger("Siapkan");

            btnSelesai = BuatTombol("✓ Selesai", new Point(225, 32), Color.FromArgb(22, 163, 74));
            btnSelesai.Click += async (s, e) => await AktifkanTrigger("AmbilDanBayar");

            btnBatalkan = BuatTombol("✗ Batalkan", new Point(330, 32), Color.FromArgb(220, 38, 38));
            btnBatalkan.Click += async (s, e) => await AktifkanTrigger("Batalkan");

            btnRefresh = BuatTombol("↻ Refresh", new Point(560, 32), Color.FromArgb(100, 116, 139));
            btnRefresh.Click += async (s, e) => await LoadPesananAsync();

            pnlAksi.Controls.AddRange(new Control[]
            {
                lblAksi, btnKonfirmasi, btnSiapkan, btnSelesai, btnBatalkan, btnRefresh
            });

            lblStatus = new Label
            {
                Text = "",
                Location = new Point(20, 512),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f)
            };

            Controls.AddRange(new Control[] { lblJudul, dgvPesanan, pnlAksi, lblStatus });
        }

        private Button BuatTombol(string text, Point loc, Color warna) => new Button
        {
            Text = text,
            Location = loc,
            Size = new Size(100, 28),
            FlatStyle = FlatStyle.Flat,
            BackColor = warna,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        private async Task LoadPesananAsync()
        {
            try
            {
                var res = await _http.GetAsync("api/pesanan");
                if (!res.IsSuccessStatusCode) return;

                var list = await res.Content.ReadFromJsonAsync<List<JsonElement>>() ?? new();
                dgvPesanan.Rows.Clear();

                foreach (var p in list)
                {
                    string namaPelanggan = "-";
                    string namaObat = "-";

                    if (p.TryGetProperty("pelanggan", out var pel) && pel.ValueKind != JsonValueKind.Null)
                        namaPelanggan = pel.GetProperty("username").GetString() ?? "-";

                    if (p.TryGetProperty("obat", out var obat) && obat.ValueKind != JsonValueKind.Null)
                        namaObat = obat.GetProperty("namaObat").GetString() ?? "-";

                    dgvPesanan.Rows.Add(
                        p.GetProperty("id"),
                        p.GetProperty("idPesanan").GetString(),
                        namaPelanggan,
                        namaObat,
                        p.GetProperty("jumlah"),
                        p.GetProperty("metodePengambilan").GetString(),
                        $"Rp{p.GetProperty("pembayaran")}",
                        p.GetProperty("status").GetString()
                    );
                }

                lblStatus.ForeColor = Color.FromArgb(100, 116, 139);
                lblStatus.Text = $"Total: {list.Count} pesanan";
            }
            catch
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Gagal memuat data.";
            }
        }

        private async Task AktifkanTrigger(string trigger)
        {
            if (dgvPesanan.CurrentRow == null)
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Pilih pesanan terlebih dahulu.";
                return;
            }

            int id = (int)dgvPesanan.CurrentRow.Cells["Id"].Value;
            try
            {
                var res = await _http.PutAsJsonAsync($"api/pesanan/{id}/trigger", new { trigger });
                if (res.IsSuccessStatusCode)
                {
                    lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
                    lblStatus.Text = "Status pesanan berhasil diperbarui.";
                    await LoadPesananAsync();
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
    }
}
