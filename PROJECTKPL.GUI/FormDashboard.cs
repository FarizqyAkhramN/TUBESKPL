using System;
using System.Drawing;
using System.Windows.Forms;
using PROJECTKPL.GUI.Services;

namespace PROJECTKPL.GUI
{
    public class FormDashboard : Form
    {
        private FlowLayoutPanel pnlSidebar;
        private Panel pnlContent;

        private Label lblJudul;
        private Label lblWelcome;

        private Button btnObat;
        private Button btnPelanggan;
        private Button btnPesanan;
        private Button btnLogout;

        private Button activeButton;

        // Facade Pattern — terima service dari FormLogin
        private readonly ObatService _obatService;
        private readonly PelangganService _pelangganService;
        private readonly PesananService _pesananService;

        public FormDashboard(ObatService obatService, PelangganService pelangganService, PesananService pesananService)
        {
            _obatService = obatService;
            _pelangganService = pelangganService;
            _pesananService = pesananService;
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

            // ================= SIDEBAR =================
            pnlSidebar = new FlowLayoutPanel
            {
                Size = new Size(200, 600),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(30, 41, 59),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true
            };

            lblJudul = new Label
            {
                Text = "ApoTek",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Margin = new Padding(20, 20, 0, 5)
            };

            lblWelcome = new Label
            {
                Text = "Selamat datang,\nAdmin",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Margin = new Padding(20, 0, 0, 10)
            };

            btnObat = CreateButton("Kelola Obat");
            btnPelanggan = CreateButton("Kelola Pelanggan");
            btnPesanan = CreateButton("Kelola Pesanan");
            btnLogout = CreateButton("Logout");

            pnlSidebar.Controls.Add(lblJudul);
            pnlSidebar.Controls.Add(lblWelcome);
            pnlSidebar.Controls.Add(new Panel { Height = 10, Width = 200 });
            pnlSidebar.Controls.Add(btnObat);
            pnlSidebar.Controls.Add(btnPelanggan);
            pnlSidebar.Controls.Add(btnPesanan);
            pnlSidebar.Controls.Add(new Panel { Height = 200, Width = 200 });
            pnlSidebar.Controls.Add(btnLogout);

            // ================= CONTENT =================
            pnlContent = new Panel
            {
                Size = new Size(700, 600),
                Location = new Point(200, 0),
                BackColor = Color.FromArgb(245, 247, 250)
            };

            var lblDefault = new Label
            {
                Text = "Pilih menu di sebelah kiri",
                Font = new Font("Segoe UI", 14f),
                ForeColor = Color.FromArgb(148, 163, 184),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlContent.Controls.Add(lblDefault);

            Controls.Add(pnlSidebar);
            Controls.Add(pnlContent);

            // ================= EVENTS =================
            // Teruskan service ke sub-form (Facade Pattern)
            btnObat.Click += (s, e) =>
            {
                SetActive(btnObat);
                LoadForm(new FormKelolaObat(_obatService));
            };

            btnPelanggan.Click += (s, e) =>
            {
                SetActive(btnPelanggan);
                LoadForm(new FormKelolaPelanggan(_pelangganService));
            };

            btnPesanan.Click += (s, e) =>
            {
                SetActive(btnPesanan);
                LoadForm(new FormKelolaPesanan(_pesananService));
            };

            btnLogout.Click += (s, e) =>
            {
                if (MessageBox.Show("Yakin ingin logout?", "Konfirmasi",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Close();
                }
            };
        }

        private Button CreateButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(200, 45),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(203, 213, 225),
                Font = new Font("Segoe UI", 9f),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 5, 0, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(51, 65, 85);
            return btn;
        }

        private void LoadForm(Form form)
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

        private void SetActive(Button btn)
        {
            if (activeButton != null)
                activeButton.BackColor = Color.Transparent;

            activeButton = btn;
            activeButton.BackColor = Color.FromArgb(51, 65, 85);
        }
    }
}