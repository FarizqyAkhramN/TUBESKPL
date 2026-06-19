using System;
using System.Collections.Generic;
using System.Text;

namespace PROJECTKPL.GUI
{
    public class FormDashboard : Form
    {
        private Panel pnlSidebar;
        private Panel pnlContent;
        private Label lblJudul;
        private Label lblWelcome;
        private Button btnObat;
        private Button btnPelanggan;
        private Button btnPesanan;
        private Button btnLogout;
        private Panel pnlContentArea;

        private readonly HttpClient _http;

        public FormDashboard(HttpClient http)
        {
            _http = http;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Dashboard Pramuniaga — ApoTek";
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

            lblJudul = new Label
            {
                Text = "ApoTek",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 24),
                AutoSize = true
            };

            lblWelcome = new Label
            {
                Text = "Selamat datang,\nAdmin",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(20, 60),
                AutoSize = true
            };

            // Separator
            var sep = new Panel
            {
                Size = new Size(160, 1),
                Location = new Point(20, 100),
                BackColor = Color.FromArgb(51, 65, 85)
            };

            btnObat = BuatTombolSidebar("🧴  Kelola Obat", 120);
            btnObat.Click += (s, e) => TampilForm(new FormKelolaObat(_http));

            btnPelanggan = BuatTombolSidebar("👤  Kelola Pelanggan", 165);
            btnPelanggan.Click += (s, e) => TampilForm(new FormKelolaPelanggan(_http));

            btnPesanan = BuatTombolSidebar("📋  Kelola Pesanan", 210);
            btnPesanan.Click += (s, e) => TampilForm(new FormKelolaPesanan(_http));

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
            btnLogout.Click += (s, e) =>
            {
                if (MessageBox.Show("Yakin ingin logout?", "Konfirmasi",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Close();
                }
            };

            pnlSidebar.Controls.AddRange(new Control[]
            {
                lblJudul, lblWelcome, sep,
                btnObat, btnPelanggan, btnPesanan, btnLogout
            });

            // ── Content area ──────────────────────────────────────────────
            pnlContent = new Panel
            {
                Size = new Size(700, 600),
                Location = new Point(200, 0),
                BackColor = Color.FromArgb(245, 247, 250)
            };

            // Tampilan awal
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
