using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PROJECTKPL.GUI
{
    public class FormMenuPelanggan : Form
    {
        private Panel pnlSidebar;
        private Panel pnlContent;
        private Label lblNama;
        private Button btnLihatObat;
        private Button btnPesanObat;
        private Button btnRiwayat;
        private Button btnGantiPassword;
        private Button btnLogout;

        private readonly HttpClient _http;
        private readonly JsonElement _pelangganData;
        private readonly int _pelangganId;
        private readonly string _username;

        public FormMenuPelanggan(HttpClient http, JsonElement pelangganData)
        {
            _http = http;
            _pelangganData = pelangganData;
            _pelangganId = pelangganData.GetProperty("id").GetInt32();
            _username = pelangganData.GetProperty("username").GetString() ?? "";
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = $"ApoTek — {_username}";
            Size = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9f);

            // ── Sidebar ───────────────────────────────────────────────────
            pnlSidebar = new Panel
            {
                Size = new Size(200, 600),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(30, 41, 59)
            };

            var lblJudul = new Label
            {
                Text = "ApoTek",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 24),
                AutoSize = true
            };

            lblNama = new Label
            {
                Text = $"Halo, {_username}!",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(20, 60),
                AutoSize = true
            };

            var sep = new Panel
            {
                Size = new Size(160, 1),
                Location = new Point(20, 90),
                BackColor = Color.FromArgb(51, 65, 85)
            };

            btnLihatObat = BuatTombolSidebar("🧴  Lihat Obat", 110);
            btnLihatObat.Click += (s, e) => TampilForm(new FormDaftarObat(_http));

            btnPesanObat = BuatTombolSidebar("🛒  Pesan Obat", 155);
            btnPesanObat.Click += (s, e) => TampilForm(new FormPesanan(_http, _pelangganId));

            btnRiwayat = BuatTombolSidebar("📋  Riwayat Pembelian", 200);
            btnRiwayat.Click += (s, e) => TampilForm(new FormRiwayat(_http, _pelangganId));

            btnGantiPassword = BuatTombolSidebar("🔑  Ganti Password", 245);
            btnGantiPassword.Click += (s, e) =>
            {
                var form = new FormGantiPassword(_http, _pelangganId,
                    _pelangganData.GetProperty("noTelp").GetString() ?? "");
                form.ShowDialog(this);
            };

            btnLogout = new Button
            {
                Text = "🚪  Logout",
                Location = new Point(0, 540),
                Size = new Size(200, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(248, 113, 113),
                Font = new Font("Segoe UI", 9f),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 65, 85);
            btnLogout.Click += (s, e) => Close();

            pnlSidebar.Controls.AddRange(new Control[]
            {
                lblJudul, lblNama, sep,
                btnLihatObat, btnPesanObat, btnRiwayat, btnGantiPassword, btnLogout
            });

            // ── Content ───────────────────────────────────────────────────
            pnlContent = new Panel
            {
                Size = new Size(700, 600),
                Location = new Point(200, 0),
                BackColor = Color.FromArgb(245, 247, 250)
            };

            var lblSelamat = new Label
            {
                Text = "Pilih menu di sebelah kiri",
                Font = new Font("Segoe UI", 14f),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = false,
                Size = new Size(700, 600),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlContent.Controls.Add(lblSelamat);

            Controls.AddRange(new Control[] { pnlSidebar, pnlContent });
        }

        private Button BuatTombolSidebar(string text, int top)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(0, top),
                Size = new Size(200, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(203, 213, 225),
                Font = new Font("Segoe UI", 9f),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 65, 85);
            return btn;
        }

        private void TampilForm(Form form)
        {
            pnlContent.Controls.Clear();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(form);
            form.Show();
        }
    }
}
