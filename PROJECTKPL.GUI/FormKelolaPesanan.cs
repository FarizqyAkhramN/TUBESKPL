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
    public class FormKelolaPesanan : Form
    {
        private Label lblJudul;
        private Label lblStatus;

        private DataGridView dgvPesanan;
        private Panel pnlAksi;

        private Button btnKonfirmasi;
        private Button btnSiapkan;
        private Button btnSelesai;
        private Button btnBatalkan;
        private Button btnRefresh;

        private readonly HttpClient _http;

        private int? selectedPesananId = null;

        public FormKelolaPesanan(HttpClient http)
        {
            _http = http;
            InitializeComponent();
            _ = LoadPesananAsync();
        }

        private void InitializeComponent()
        {
            Text = "Kelola Pesanan";
            Size = new Size(720, 600);
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9f);

            // ================= TITLE =================
            lblJudul = new Label
            {
                Text = "Kelola Pesanan",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 15),
                AutoSize = true
            };

            // ================= GRID =================
            dgvPesanan = new DataGridView
            {
                Location = new Point(20, 55),
                Size = new Size(660, 360),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvPesanan.Columns.Add("Id", "ID");
            dgvPesanan.Columns.Add("IdPesanan", "ID Pesanan");
            dgvPesanan.Columns.Add("Pelanggan", "Pelanggan");
            dgvPesanan.Columns.Add("Obat", "Obat");
            dgvPesanan.Columns.Add("Jumlah", "Jml");
            dgvPesanan.Columns.Add("Metode", "Metode");
            dgvPesanan.Columns.Add("Pembayaran", "Pembayaran");
            dgvPesanan.Columns.Add("Status", "Status");

            dgvPesanan.CellClick += DgvPesanan_CellClick;

            // ================= ACTION PANEL =================
            pnlAksi = new Panel
            {
                Location = new Point(20, 430),
                Size = new Size(660, 80),
                BackColor = Color.White
            };

            pnlAksi.Paint += (s, e) =>
            {
                e.Graphics.DrawRectangle(
                    new Pen(Color.FromArgb(226, 232, 240)),
                    0, 0, pnlAksi.Width - 1, pnlAksi.Height - 1
                );
            };

            var lblAksi = new Label
            {
                Text = "Pilih pesanan dulu sebelum aksi:",
                Location = new Point(15, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85)
            };

            btnKonfirmasi = CreateButton("Konfirmasi", new Point(15, 35), Color.FromArgb(37, 99, 235));
            btnSiapkan = CreateButton("Siapkan", new Point(120, 35), Color.FromArgb(234, 179, 8));
            btnSelesai = CreateButton("Selesai", new Point(225, 35), Color.FromArgb(22, 163, 74));
            btnBatalkan = CreateButton("Batalkan", new Point(330, 35), Color.FromArgb(220, 38, 38));
            btnRefresh = CreateButton("Refresh", new Point(560, 35), Color.FromArgb(100, 116, 139));

            btnKonfirmasi.Click += async (s, e) => await Trigger("Konfirmasi");
            btnSiapkan.Click += async (s, e) => await Trigger("Siapkan");
            btnSelesai.Click += async (s, e) => await Trigger("AmbilDanBayar");
            btnBatalkan.Click += async (s, e) => await Trigger("Batalkan");
            btnRefresh.Click += async (s, e) => await LoadPesananAsync();

            pnlAksi.Controls.AddRange(new Control[]
            {
                lblAksi,
                btnKonfirmasi,
                btnSiapkan,
                btnSelesai,
                btnBatalkan,
                btnRefresh
            });

            // ================= STATUS =================
            lblStatus = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 30,
                Padding = new Padding(10, 5, 0, 0),
                ForeColor = Color.FromArgb(100, 116, 139),
                Text = "READY"
            };

            Controls.AddRange(new Control[]
            {
                lblJudul,
                dgvPesanan,
                pnlAksi,
                lblStatus
            });
        }

        // ================= GRID CLICK =================
        private void DgvPesanan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvPesanan.Rows[e.RowIndex];

            selectedPesananId = Convert.ToInt32(row.Cells["Id"].Value);

            lblStatus.Text = $"Selected ID: {selectedPesananId}";
        }

        // ================= LOAD =================
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
                    string pelanggan = "-";
                    string obat = "-";

                    if (p.TryGetProperty("pelanggan", out var pel))
                        pelanggan = pel.GetProperty("username").GetString() ?? "-";

                    if (p.TryGetProperty("obat", out var ob))
                        obat = ob.GetProperty("namaObat").GetString() ?? "-";

                    dgvPesanan.Rows.Add(
                        p.GetProperty("id").GetInt32(),
                        p.GetProperty("idPesanan").GetString(),
                        pelanggan,
                        obat,
                        p.GetProperty("jumlah").GetInt32(),
                        p.GetProperty("metodePengambilan").GetString(),
                        $"Rp{p.GetProperty("pembayaran").GetInt32()}",
                        p.GetProperty("status").GetString()
                    );
                }

                lblStatus.Text = $"Total: {list.Count} pesanan";
            }
            catch
            {
                lblStatus.Text = "Gagal load data.";
            }
        }

        // ================= TRIGGER STATE MACHINE =================
        private async Task Trigger(string trigger)
        {
            if (selectedPesananId == null)
            {
                lblStatus.Text = "Pilih pesanan dulu.";
                return;
            }

            try
            {
                var res = await _http.PutAsJsonAsync(
                    $"api/pesanan/{selectedPesananId}/trigger",
                    new { trigger }
                );

                if (res.IsSuccessStatusCode)
                {
                    lblStatus.Text = $"Status updated: {trigger}";
                    await LoadPesananAsync();
                }
                else
                {
                    lblStatus.Text = "Gagal update status.";
                }
            }
            catch
            {
                lblStatus.Text = "Server tidak bisa diakses.";
            }
        }

        // ================= BUTTON FACTORY =================
        private Button CreateButton(string text, Point loc, Color color)
        {
            return new Button
            {
                Text = text,
                Location = loc,
                Size = new Size(100, 28),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }
    }
}