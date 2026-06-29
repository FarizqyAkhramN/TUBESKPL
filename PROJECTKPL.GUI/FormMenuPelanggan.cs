using System;
using System.Drawing;
using System.Text.Json;
using System.Windows.Forms;
using PROJECTKPL.GUI.Services;

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
        private Button? btnAktif;

        // Facade Pattern — terima service dari FormLogin
        private readonly ObatService _obatService;
        private readonly PelangganService _pelangganService;
        private readonly PesananService _pesananService;
        private readonly JsonElement _pelangganData;
        private readonly int _pelangganId;
        private readonly string _username;

        public FormMenuPelanggan(ObatService obatService, PelangganService pelangganService,
            PesananService pesananService, JsonElement pelangganData)
        {
            _obatService = obatService;
            _pelangganService = pelangganService;
            _pesananService = pesananService;
            _pelangganData = pelangganData;
            _pelangganId = pelangganData.GetProperty("id").GetInt32();
            _username = pelangganData.GetProperty("username").GetString() ?? "";

            InitializeComponent();

            // Menu default
            SetMenuAktif(btnLihatObat);
            TampilForm(new FormDaftarObat(_obatService));
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

            // =====================================================
            // SIDEBAR
            // =====================================================
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
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
                Size = new Size(180, 1),
                Location = new Point(20, 95),
                BackColor = Color.FromArgb(51, 65, 85)
            };

            btnLihatObat = BuatTombolSidebar("Lihat Obat", 115);
            btnPesanObat = BuatTombolSidebar("Pesan Obat", 160);
            btnRiwayat = BuatTombolSidebar("Riwayat Pembelian", 205);
            btnGantiPassword = BuatTombolSidebar("Ganti Password", 250);

            // Facade Pattern — teruskan service ke sub-form
            btnLihatObat.Click += (s, e) =>
            {
                SetMenuAktif(btnLihatObat);
                TampilForm(new FormDaftarObat(_obatService));
            };

            btnPesanObat.Click += (s, e) =>
            {
                SetMenuAktif(btnPesanObat);
                TampilForm(new FormPesanan(_obatService, _pesananService, _pelangganId));
            };

            btnRiwayat.Click += (s, e) =>
            {
                SetMenuAktif(btnRiwayat);
                TampilForm(new FormRiwayat(_pesananService, _pelangganId));
            };

            btnGantiPassword.Click += (s, e) =>
            {
                SetMenuAktif(btnGantiPassword);
                using var form = new FormGantiPassword(
                    _pelangganService,
                    _pelangganId,
                    _pelangganData.GetProperty("noTelp").GetString() ?? ""
                );
                form.ShowDialog(this);
            };

            btnLogout = new Button
            {
                Text = "Logout",
                Size = new Size(220, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(248, 113, 113),
                Font = new Font("Segoe UI", 9f),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 65, 85);
            btnLogout.Location = new Point(0, pnlSidebar.Height - 50);
            pnlSidebar.Resize += (s, e) =>
            {
                btnLogout.Location = new Point(0, pnlSidebar.Height - 50);
            };
            btnLogout.Click += (s, e) =>
            {
                if (MessageBox.Show("Yakin ingin logout?", "Konfirmasi Logout",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    Close();
            };

            pnlSidebar.Controls.AddRange(new Control[]
            {
                lblJudul, lblNama, sep,
                btnLihatObat, btnPesanObat, btnRiwayat, btnGantiPassword, btnLogout
            });

            // =====================================================
            // CONTENT
            // =====================================================
            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(245, 247, 250)
            };

            Controls.Add(pnlContent);
            Controls.Add(pnlSidebar);
        }

        private Button BuatTombolSidebar(string text, int top)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(0, top),
                Size = new Size(220, 42),
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

        private void SetMenuAktif(Button btn)
        {
            if (btnAktif != null)
            {
                btnAktif.BackColor = Color.Transparent;
                btnAktif.ForeColor = Color.FromArgb(203, 213, 225);
            }
            btn.BackColor = Color.FromArgb(51, 65, 85);
            btn.ForeColor = Color.White;
            btnAktif = btn;
        }

        private void TampilForm(Form form)
        {
            foreach (Control c in pnlContent.Controls)
                c.Dispose();
            pnlContent.Controls.Clear();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(form);
            form.Show();
        }
    }
}