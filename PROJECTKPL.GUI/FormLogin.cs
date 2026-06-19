using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace PROJECTKPL.GUI
{
    public class FormLogin : Form
    {
        // ── Controls ───────────────────────────────────────────────────────
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

        private readonly HttpClient _http;

        public FormLogin()
        {
            _http = new HttpClient { BaseAddress = new Uri("http://localhost:5252/") };
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // ── Form ──────────────────────────────────────────────────────
            Text = "PROJECTKPL — Sistem Apotek";
            Size = new Size(420, 560);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9f);

            // ── Card panel ────────────────────────────────────────────────
            pnlCard = new Panel
            {
                Size = new Size(340, 460),
                Location = new Point(40, 40),
                BackColor = Color.White,
                Padding = new Padding(30)
            };
            pnlCard.Paint += (s, e) =>
            {
                e.Graphics.DrawRectangle(
                    new Pen(Color.FromArgb(220, 220, 220), 1),
                    0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
            };

            // ── Judul ─────────────────────────────────────────────────────
            lblJudul = new Label
            {
                Text = "ApoTek",
                Font = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235),
                Location = new Point(30, 30),
                AutoSize = true
            };

            lblSubjudul = new Label
            {
                Text = "Sistem Manajemen Apotek",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(30, 70),
                AutoSize = true
            };

            // ── Role ──────────────────────────────────────────────────────
            lblRole = new Label
            {
                Text = "Login sebagai",
                Location = new Point(30, 115),
                AutoSize = true,
                ForeColor = Color.FromArgb(51, 65, 85),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };

            cmbRole = new ComboBox
            {
                Location = new Point(30, 135),
                Size = new Size(280, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f)
            };
            cmbRole.Items.AddRange(new object[] { "Pramuniaga / Admin", "Pelanggan" });
            cmbRole.SelectedIndex = 0;
            cmbRole.SelectedIndexChanged += OnRoleChanged;

            // ── Username / No Telp ────────────────────────────────────────
            lblUsername = new Label
            {
                Text = "Username",
                Location = new Point(30, 180),
                AutoSize = true,
                ForeColor = Color.FromArgb(51, 65, 85),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };

            txtUsername = new TextBox
            {
                Location = new Point(30, 200),
                Size = new Size(280, 28),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9f)
            };

            // ── Password ──────────────────────────────────────────────────
            lblPassword = new Label
            {
                Text = "Password",
                Location = new Point(30, 245),
                AutoSize = true,
                ForeColor = Color.FromArgb(51, 65, 85),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };

            txtPassword = new TextBox
            {
                Location = new Point(30, 265),
                Size = new Size(280, 28),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9f),
                PasswordChar = '●',
                UseSystemPasswordChar = false
            };
            txtPassword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) BtnLogin_Click(s, e);
            };

            // ── Error label ───────────────────────────────────────────────
            lblError = new Label
            {
                Text = "",
                Location = new Point(30, 305),
                Size = new Size(280, 20),
                ForeColor = Color.FromArgb(220, 38, 38),
                Font = new Font("Segoe UI", 8.5f),
                AutoSize = false
            };

            // ── Tombol Login ──────────────────────────────────────────────
            btnLogin = new Button
            {
                Text = "Masuk",
                Location = new Point(30, 330),
                Size = new Size(280, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            // ── Tombol Daftar ─────────────────────────────────────────────
            btnDaftar = new Button
            {
                Text = "Belum punya akun? Daftar",
                Location = new Point(30, 380),
                Size = new Size(280, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(37, 99, 235),
                Font = new Font("Segoe UI", 9f),
                Cursor = Cursors.Hand,
                Visible = false
            };
            btnDaftar.FlatAppearance.BorderSize = 0;
            btnDaftar.Click += BtnDaftar_Click;

            // ── Susun controls ────────────────────────────────────────────
            pnlCard.Controls.AddRange(new Control[]
            {
                lblJudul, lblSubjudul,
                lblRole, cmbRole,
                lblUsername, txtUsername,
                lblPassword, txtPassword,
                lblError, btnLogin, btnDaftar
            });

            Controls.Add(pnlCard);
        }

        // ── Event: ganti role ──────────────────────────────────────────────
        private void OnRoleChanged(object? sender, EventArgs e)
        {
            bool isPelanggan = cmbRole.SelectedIndex == 1;
            lblUsername.Text = isPelanggan ? "No. Telepon" : "Username";
            btnDaftar.Visible = isPelanggan;
            lblError.Text = "";
        }

        // ── Event: tombol login ────────────────────────────────────────────
        private async void BtnLogin_Click(object? sender, EventArgs e)
        {
            lblError.Text = "";
            string inputIdentitas = txtUsername.Text.Trim();
            string inputPassword = txtPassword.Text;

            // Validasi input kosong
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
                    // Login Pramuniaga — cek hardcode (sesuai CLO2)
                    if (inputIdentitas == "admin" && inputPassword == "admin123")
                    {
                        var dashboard = new FormDashboard(_http);
                        Hide();
                        dashboard.FormClosed += (s, args) => Close();
                        dashboard.Show();
                    }
                    else
                    {
                        lblError.Text = "Username atau password salah.";
                    }
                }
                else
                {
                    // Login Pelanggan — hit API
                    var res = await _http.PostAsJsonAsync("api/pelanggan/login",
                        new { noTelp = inputIdentitas, password = inputPassword });

                    if (res.IsSuccessStatusCode)
                    {
                        var json = await res.Content.ReadFromJsonAsync<JsonElement>();
                        var formMenu = new FormMenuPelanggan(_http, json);
                        Hide();
                        formMenu.FormClosed += (s, args) => Close();
                        formMenu.Show();
                    }
                    else
                    {
                        lblError.Text = "No. telepon atau password salah.";
                    }
                }
            }
            catch (Exception)
            {
                lblError.Text = "Tidak dapat terhubung ke server.";
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "Masuk";
            }
        }

        // ── Event: tombol daftar ───────────────────────────────────────────
        private void BtnDaftar_Click(object? sender, EventArgs e)
        {
            var formDaftar = new FormRegistrasi(_http);
            formDaftar.ShowDialog(this);
        }

    }
