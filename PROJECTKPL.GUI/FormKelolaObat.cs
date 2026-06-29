using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using PROJECTKPL.GUI.Services;

namespace PROJECTKPL.GUI
{
    public class FormKelolaObat : Form
    {
        private enum Mode { Add, Edit }
        private Mode currentMode = Mode.Add;

        private Label lblJudul;
        private Label lblStatus;
        private DataGridView dgvObat;
        private Panel pnlForm;
        private TextBox txtNama;
        private TextBox txtStok;
        private TextBox txtHarga;
        private Button btnTambah;
        private Button btnEditStok;
        private Button btnHapus;
        private Button btnRefresh;

        // Facade Pattern — pakai ObatService bukan HttpClient langsung
        private readonly ObatService _obatService;
        private int? selectedObatId = null;

        public FormKelolaObat(ObatService obatService)
        {
            _obatService = obatService;
            InitializeComponent();
            _ = LoadObatAsync();
        }

        private void InitializeComponent()
        {
            Text = "Kelola Obat";
            Size = new Size(720, 600);
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9f);

            // ================= TITLE =================
            lblJudul = new Label
            {
                Text = "Kelola Obat",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 15),
                AutoSize = true
            };

            // ================= GRID =================
            dgvObat = new DataGridView
            {
                Location = new Point(20, 55),
                Size = new Size(660, 260),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvObat.Columns.Add("Id", "ID");
            dgvObat.Columns.Add("NamaObat", "Nama Obat");
            dgvObat.Columns.Add("Stok", "Stok");
            dgvObat.Columns.Add("Status", "Status");
            dgvObat.Columns.Add("Harga", "Harga");
            dgvObat.CellClick += DgvObat_CellClick;

            // ================= FORM =================
            pnlForm = new Panel
            {
                Location = new Point(20, 330),
                Size = new Size(660, 170),
                BackColor = Color.White
            };
            pnlForm.Paint += (s, e) =>
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(226, 232, 240)),
                    0, 0, pnlForm.Width - 1, pnlForm.Height - 1);

            var lblForm = new Label
            {
                Text = "Tambah / Edit Obat",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Location = new Point(15, 10),
                AutoSize = true
            };

            txtNama = new TextBox { Location = new Point(15, 55), Size = new Size(200, 28) };
            txtStok = new TextBox { Location = new Point(225, 55), Size = new Size(120, 28) };
            txtHarga = new TextBox { Location = new Point(355, 55), Size = new Size(120, 28) };

            var lblNama = new Label { Text = "Nama", Location = new Point(15, 35), AutoSize = true };
            var lblStok = new Label { Text = "Stok", Location = new Point(225, 35), AutoSize = true };
            var lblHarga = new Label { Text = "Harga", Location = new Point(355, 35), AutoSize = true };

            btnTambah = CreateButton("Tambah", Color.FromArgb(37, 99, 235), 15);
            btnEditStok = CreateButton("Edit", Color.FromArgb(234, 179, 8), 115);
            btnHapus = CreateButton("Hapus", Color.FromArgb(220, 38, 38), 215);
            btnRefresh = CreateButton("Refresh", Color.FromArgb(100, 116, 139), 315);

            btnTambah.Click += BtnTambah_Click;
            btnEditStok.Click += BtnEditStok_Click;
            btnHapus.Click += BtnHapus_Click;
            btnRefresh.Click += async (s, e) => await RefreshAll();

            pnlForm.Controls.AddRange(new Control[]
            {
                lblForm,
                lblNama, lblStok, lblHarga,
                txtNama, txtStok, txtHarga,
                btnTambah, btnEditStok, btnHapus, btnRefresh
            });

            // ================= STATUS =================
            lblStatus = new Label
            {
                Location = new Point(20, 520),
                AutoSize = true,
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            Controls.AddRange(new Control[] { lblJudul, dgvObat, pnlForm, lblStatus });
        }

        // ================= GRID CLICK =================
        private void DgvObat_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvObat.Rows[e.RowIndex];
            selectedObatId = Convert.ToInt32(row.Cells["Id"].Value);
            txtNama.Text = row.Cells["NamaObat"].Value?.ToString();
            txtStok.Text = row.Cells["Stok"].Value?.ToString();
            txtHarga.Text = row.Cells["Harga"].Value?.ToString()?.Replace("Rp", "");
            currentMode = Mode.Edit;
            lblStatus.ForeColor = Color.FromArgb(37, 99, 235);
            lblStatus.Text = "MODE: EDIT";
        }

        // ================= LOAD =================
        private async Task LoadObatAsync()
        {
            try
            {
                // Facade Pattern — cukup panggil service
                var data = await _obatService.GetAllAsync();
                dgvObat.Rows.Clear();
                foreach (var o in data)
                {
                    dgvObat.Rows.Add(
                        o.GetProperty("id").GetInt32(),
                        o.GetProperty("namaObat").GetString(),
                        o.GetProperty("stok").GetInt32(),
                        o.GetProperty("statusObat").GetString(),
                        $"Rp{o.GetProperty("harga").GetInt32()}"
                    );
                }
                lblStatus.ForeColor = Color.FromArgb(100, 116, 139);
                lblStatus.Text = $"Total: {data.Count} obat | MODE: {currentMode}";
                dgvObat.ClearSelection();
                try { dgvObat.CurrentCell = null; } catch { }
            }
            catch
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Gagal load data.";
            }
        }

        // ================= TAMBAH =================
        private async void BtnTambah_Click(object sender, EventArgs e)
        {
            if (currentMode == Mode.Edit)
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Selesaikan edit dulu (Refresh untuk kembali ke mode tambah).";
                return;
            }
            if (!Validate(out int stok, out int harga)) return;

            string namaBaru = txtNama.Text.Trim();

            // Cek duplikat nama dari grid
            foreach (DataGridViewRow row in dgvObat.Rows)
            {
                if (row.IsNewRow) continue;
                string namaExisting = row.Cells["NamaObat"].Value?.ToString()?.Trim() ?? "";
                if (namaExisting.Equals(namaBaru, StringComparison.OrdinalIgnoreCase))
                {
                    lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                    lblStatus.Text = "Nama obat sudah terdaftar.";
                    return;
                }
            }

            var (sukses, pesan) = await _obatService.TambahAsync(namaBaru, stok, harga);
            if (sukses)
            {
                lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
                lblStatus.Text = "Obat berhasil ditambahkan.";
                await RefreshAll();
            }
            else
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = $"Gagal: {pesan}";
            }
        }

        // ================= EDIT =================
        private async void BtnEditStok_Click(object? sender, EventArgs e)
        {
            if (selectedObatId == null)
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Pilih obat yang ingin diedit.";
                return;
            }
            if (!int.TryParse(txtStok.Text, out int stokBaru) || stokBaru < 0)
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
            if (string.IsNullOrWhiteSpace(txtNama.Text))
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Nama obat harus diisi.";
                return;
            }

            var (sukses, pesan) = await _obatService.EditAsync(selectedObatId.Value, txtNama.Text.Trim(), stokBaru, harga);
            if (sukses)
            {
                lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
                lblStatus.Text = "Obat berhasil diperbarui.";
                await RefreshAll();
            }
            else
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = $"Gagal: {pesan}";
            }
        }

        // ================= HAPUS =================
        private async void BtnHapus_Click(object sender, EventArgs e)
        {
            if (selectedObatId == null)
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Pilih obat dulu.";
                return;
            }

            if (MessageBox.Show("Yakin ingin menghapus obat ini?", "Konfirmasi Hapus",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var (sukses, _) = await _obatService.HapusAsync(selectedObatId.Value);
            if (sukses)
            {
                lblStatus.ForeColor = Color.FromArgb(22, 163, 74);
                lblStatus.Text = "Obat berhasil dihapus.";
                await RefreshAll();
            }
            else
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Gagal menghapus obat.";
            }
        }

        // ================= REFRESH =================
        private async Task RefreshAll()
        {
            ResetForm();
            await LoadObatAsync();
        }

        private void ResetForm()
        {
            selectedObatId = null;
            currentMode = Mode.Add;
            dgvObat.ClearSelection();
            try { dgvObat.CurrentCell = null; } catch { }
            txtNama.Text = "";
            txtStok.Text = "";
            txtHarga.Text = "";
            lblStatus.ForeColor = Color.FromArgb(100, 116, 139);
            lblStatus.Text = "MODE: ADD";
        }

        // ================= VALIDATION =================
        private bool Validate(out int stok, out int harga)
        {
            stok = harga = 0;
            if (string.IsNullOrWhiteSpace(txtNama.Text)) { lblStatus.Text = "Nama wajib diisi."; return false; }
            if (!int.TryParse(txtStok.Text, out stok)) { lblStatus.Text = "Stok tidak valid."; return false; }
            if (!int.TryParse(txtHarga.Text, out harga)) { lblStatus.Text = "Harga tidak valid."; return false; }
            return true;
        }

        private Button CreateButton(string text, Color color, int left) => new Button
        {
            Text = text,
            Location = new Point(left, 105),
            Size = new Size(90, 32),
            BackColor = color,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold)
        };
    }
}