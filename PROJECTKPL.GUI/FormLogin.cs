using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using PROJECTKPL.GUI.Services;

namespace PROJECTKPL.GUI
{
    public class FormLogin : Form
    {
        private Panel pnlCard;
        private Label lblJudul;
        private Label lblSubjudul;
        private Label lblRole;
        private ComboBox cmbRole;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblPassword;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnDaftar;
        private Label lblError;

        // Facade Pattern — service dibuat sekali di sini, diteruskan ke semua form
        private readonly PelangganService _pelangganService;
        private readonly ObatService _obatService;
        private readonly PesananService _pesananService;

        public FormLogin()
        {
            var http = new HttpClient { BaseAddress = new Uri("http://localhost:5188/") };

            _pelangganService = new PelangganService(http);
            _obatService = new ObatService(http);
            _pesananService = new PesananService(http);

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Sistem Apotek";
            Size = new Size(500, 400);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(240, 240, 240);
            Font = new Font("Segoe UI", 9F);

            lblJudul = new Label
            {
                Text = "LOGIN SISTEM APOTEK",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(75, 20),
                ForeColor = Color.Black
            };

            lblSubjudul = new Label
            {
                Text = "Masukkan akun untuk melanjutkan",
                AutoSize = true,
                Location = new Point(90, 50),
                ForeColor = Color.DimGray
            };

            lblRole = new Label
            {
                Text = "Role",
                AutoSize = true,
                Location = new Point(50, 95)
            };

            cmbRole = new ComboBox
            {
                Location = new Point(50, 115),
                Size = new Size(380, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRole.Items.AddRange(new object[] { "Pramuniaga / Admin", "Pelanggan" });
            cmbRole.SelectedIndex = 0;
            cmbRole.SelectedIndexChanged += OnRoleChanged;

            lblUsername = new Label
            {
                Text = "Username",
                AutoSize = true,
                Location = new Point(50, 155)
            };

            txtUsername = new TextBox
            {
                Location = new Point(50, 175),
                Size = new Size(380, 27)
            };

            lblPassword = new Label
            {
                Text = "Password",
                AutoSize = true,
                Location = new Point(50, 215)
            };

            txtPassword = new TextBox
            {
                Location = new Point(50, 235),
                Size = new Size(380, 27),
                PasswordChar = '*'
            };
            txtPassword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) BtnLogin_Click(s, e);
            };

            lblError = new Label
            {
                AutoSize = false,
                Size = new Size(380, 20),
                Location = new Point(50, 270),
                ForeColor = Color.DarkRed
            };

            btnLogin = new Button
            {
                Text = "Login",
                Size = new Size(180, 35),
                Location = new Point(50, 300),
                BackColor = Color.Gainsboro
            };
            btnLogin.Click += BtnLogin_Click;

            btnDaftar = new Button
            {
                Text = "Daftar Akun",
                Size = new Size(180, 35),
                Location = new Point(250, 300),
                BackColor = Color.Gainsboro,
                Visible = false
            };
            btnDaftar.Click += BtnDaftar_Click;

            Controls.AddRange(new Control[]
            {
                lblJudul, lblSubjudul,
                lblRole, cmbRole,
                lblUsername, txtUsername,
                lblPassword, txtPassword,
                lblError,
                btnLogin, btnDaftar
            });
        }

        private void OnRoleChanged(object? sender, EventArgs e)
        {
            bool isPelanggan = cmbRole.SelectedIndex == 1;
            lblUsername.Text = isPelanggan ? "No. Telepon" : "Username";
            btnDaftar.Visible = isPelanggan;
            lblError.Text = "";
        }

        private async void BtnLogin_Click(object? sender, EventArgs e)
        {
            lblError.Text = "";
            string inputIdentitas = txtUsername.Text.Trim();
            string inputPassword = txtPassword.Text;

            if (string.IsNullOrEmpty(inputIdentitas) || string.IsNullOrEmpty(inputPassword))
            {
                lblError.Text = "Semua field harus diisi.";
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "Memproses...";

            try
            {
                if (cmbRole.SelectedIndex == 0)
                {
                    if (inputIdentitas == "admin" && inputPassword == "admin123")
                    {
                        // Teruskan service ke FormDashboard (Facade Pattern)
                        var dashboard = new FormDashboard(_obatService, _pelangganService, _pesananService);
                        Hide();
                        dashboard.FormClosed += (s, args) =>
                        {
                            txtUsername.Clear();
                            txtPassword.Clear();
                            lblError.Text = "";
                            Show();
                        };
                        dashboard.Show();
                    }
                    else
                    {
                        lblError.Text = "Username atau password salah.";
                    }
                }
                else
                {
                    // Login Pelanggan — pakai service bukan HttpClient langsung
                    var (sukses, data) = await _pelangganService.LoginAsync(inputIdentitas, inputPassword);
                    if (sukses && data.HasValue)
                    {
                        var formMenu = new FormMenuPelanggan(_obatService, _pelangganService, _pesananService, data.Value);
                        Hide();
                        formMenu.FormClosed += (s, args) =>
                        {
                            txtUsername.Clear();
                            txtPassword.Clear();
                            lblError.Text = "";
                            Show();
                        };
                        formMenu.Show();
                    }
                    else
                    {
                        lblError.Text = "No. telepon atau password salah.";
                    }
                }
            }
            catch
            {
                lblError.Text = "Tidak dapat terhubung ke server.";
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "Masuk";
            }
        }

        private void BtnDaftar_Click(object? sender, EventArgs e)
        {
            var formDaftar = new FormRegistrasi(_pelangganService);
            formDaftar.ShowDialog(this);
        }
    }
}