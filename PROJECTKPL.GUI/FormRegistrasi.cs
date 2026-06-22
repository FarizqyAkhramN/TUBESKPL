using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace PROJECTKPL.GUI
{
    public class FormRegistrasi : Form
    {
        private Label lblJudul;
        private TextBox txtUsername;
        private ComboBox cmbGender;
        private TextBox txtNoTelp;
        private TextBox txtUmur;
        private TextBox txtPassword;
        private TextBox txtKonfirmPassword;
        private Button btnDaftar;
        private Button btnBatal;
        private Label lblStatus;

        private readonly HttpClient _http;

        public FormRegistrasi(HttpClient http)
        {
            _http = http;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Registrasi Akun";
            Size = new Size(500, 620);

            StartPosition = FormStartPosition.CenterParent;

            FormBorderStyle = FormBorderStyle.FixedDialog;

            MaximizeBox = false;
            MinimizeBox = false;

            BackColor = Color.FromArgb(240, 240, 240);

            Font = new Font("Segoe UI", 9F);

            lblJudul = new Label
            {
                Text = "REGISTRASI AKUN",
                Font = new Font(
                    "Segoe UI",
                    14,
                    FontStyle.Bold
                ),
                AutoSize = true,
                Location = new Point(150, 20)
            };

            int left = 40;
            int width = 390;
            int top = 70;

            Controls.Add(lblJudul);

            Controls.Add(BuatLabel("Username", top));
            txtUsername = BuatTextBox(top + 20);
            txtUsername.Width = width;
            Controls.Add(txtUsername);

            top += 60;

            Controls.Add(BuatLabel("Gender", top));

            cmbGender = new ComboBox
            {
                Location = new Point(20, top + 20),
                Size = new Size(width, 27),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cmbGender.Items.AddRange(new object[]
            {
        "Laki-laki",
        "Perempuan"
            });

            cmbGender.SelectedIndex = 0;

            Controls.Add(cmbGender);

            top += 60;

            Controls.Add(BuatLabel("No Telepon", top));

            txtNoTelp = BuatTextBox(top + 20);
            txtNoTelp.Width = width;

            Controls.Add(txtNoTelp);

            top += 60;

            Controls.Add(BuatLabel("Umur", top));

            txtUmur = BuatTextBox(top + 20);
            txtUmur.Width = width;

            Controls.Add(txtUmur);

            top += 60;

            Controls.Add(BuatLabel("Password", top));

            txtPassword = BuatTextBox(top + 20);
            txtPassword.Width = width;
            txtPassword.PasswordChar = '*';

            Controls.Add(txtPassword);

            top += 60;

            Controls.Add(BuatLabel("Konfirmasi Password", top));

            txtKonfirmPassword = BuatTextBox(top + 20);
            txtKonfirmPassword.Width = width;
            txtKonfirmPassword.PasswordChar = '*';

            Controls.Add(txtKonfirmPassword);

            top += 70;

            lblStatus = new Label
            {
                Location = new Point(left, top),
                Size = new Size(width, 25),
                ForeColor = Color.DarkRed
            };

            Controls.Add(lblStatus);

            top += 40;

            btnDaftar = new Button
            {
                Text = "Daftar",
                Size = new Size(185, 35),
                Location = new Point(left, top),
                BackColor = Color.Gainsboro
            };

            btnDaftar.Click += BtnDaftar_Click;

            btnBatal = new Button
            {
                Text = "Batal",
                Size = new Size(185, 35),
                Location = new Point(left + 205, top),
                BackColor = Color.Gainsboro
            };

            btnBatal.Click += (s, e) => Close();

            Controls.Add(btnDaftar);
            Controls.Add(btnBatal);
        }

        private Label BuatLabel(string text, int top) => new Label
        {
            Text = text,
            Location = new Point(20, top),
            AutoSize = true,
            ForeColor = Color.FromArgb(71, 85, 105),
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
        };

        private TextBox BuatTextBox(int top) => new TextBox
        {
            Location = new Point(20, top),
            Size = new Size(300, 26),
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Segoe UI", 9f)
        };

        private async void BtnDaftar_Click(object? sender, EventArgs e)
        {
            lblStatus.ForeColor = Color.FromArgb(220, 38, 38);

            // ── Validasi Username ─────────────────────────────────────────────
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                lblStatus.Text = "Username harus diisi.";
                txtUsername.Focus();
                return;
            }
            if (txtUsername.Text.Trim().Length < 3)
            {
                lblStatus.Text = "Username minimal 3 karakter.";
                txtUsername.Focus();
                return;
            }
            if (txtUsername.Text.Trim().Length > 50)
            {
                lblStatus.Text = "Username maksimal 50 karakter.";
                txtUsername.Focus();
                return;
            }

            // ── Validasi No. Telepon ──────────────────────────────────────────
            string noTelp = txtNoTelp.Text.Trim();
            if (string.IsNullOrWhiteSpace(noTelp))
            {
                lblStatus.Text = "No. telepon harus diisi.";
                txtNoTelp.Focus();
                return;
            }
            if (!noTelp.StartsWith("08"))
            {
                lblStatus.Text = "No. telepon harus diawali dengan 08.";
                txtNoTelp.Focus();
                return;
            }
            if (noTelp.Length < 10 || noTelp.Length > 13)
            {
                lblStatus.Text = "No. telepon harus terdiri dari 10 sampai 13 digit.";
                txtNoTelp.Focus();
                return;
            }
            if (!noTelp.All(char.IsDigit))
            {
                lblStatus.Text = "No. telepon hanya boleh berisi angka.";
                txtNoTelp.Focus();
                return;
            }

            // ── Validasi Umur ─────────────────────────────────────────────────
            if (!int.TryParse(txtUmur.Text, out int umur))
            {
                lblStatus.Text = "Umur harus berupa angka.";
                txtUmur.Focus();
                return;
            }
            if (umur < 1 || umur > 120)
            {
                lblStatus.Text = "Umur harus antara 1 sampai 120 tahun.";
                txtUmur.Focus();
                return;
            }

            // ── Validasi Password ─────────────────────────────────────────────
            string password = txtPassword.Text;
            if (string.IsNullOrEmpty(password))
            {
                lblStatus.Text = "Password harus diisi.";
                txtPassword.Focus();
                return;
            }
            if (password.Length < 6)
            {
                lblStatus.Text = "Password minimal 6 karakter.";
                txtPassword.Focus();
                return;
            }
            if (!password.Any(char.IsUpper))
            {
                lblStatus.Text = "Password harus mengandung 1 huruf kapital.";
                txtPassword.Focus();
                return;
            }
            if (!password.Any(char.IsDigit))
            {
                lblStatus.Text = "Password harus mengandung minimal 1 angka.";
                txtPassword.Focus();
                return;
            }

            // ── Validasi Konfirmasi Password ──────────────────────────────────
            if (password != txtKonfirmPassword.Text)
            {
                lblStatus.Text = "Konfirmasi password tidak cocok.";
                txtKonfirmPassword.Focus();
                return;
            }

            // ── Semua valid — kirim ke API ────────────────────────────────────
            btnDaftar.Enabled = false;
            btnDaftar.Text = "Mendaftarkan...";

            try
            {
                var res = await _http.PostAsJsonAsync("api/pelanggan", new
                {
                    username = txtUsername.Text.Trim(),
                    gender = cmbGender.SelectedItem?.ToString() ?? "",
                    noTelp,
                    umur,
                    password
                });

                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Akun berhasil dibuat!\nSilakan login dengan akun baru Anda.",
                        "Registrasi Berhasil",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    Close();
                }
                else
                {
                    string msg = await res.Content.ReadAsStringAsync();
                    try
                    {
                        // Parse error dari FluentValidation (array JSON)
                        var errors = System.Text.Json.JsonSerializer.Deserialize<
                            List<System.Text.Json.JsonElement>>(msg);
                        if (errors != null && errors.Count > 0)
                            lblStatus.Text = errors[0].GetProperty("errorMessage").GetString()
                                ?? "Pendaftaran gagal.";
                        else
                            lblStatus.Text = "Pendaftaran gagal.";
                    }
                    catch
                    {
                        // Jika bukan JSON (misal conflict no telp)
                        lblStatus.Text = msg.Length > 100
                            ? "Pendaftaran gagal. Cek kembali data Anda."
                            : msg.Trim('"');
                    }
                }
            }
            catch
            {
                lblStatus.Text = "Tidak dapat terhubung ke server.";
            }
            finally
            {
                btnDaftar.Enabled = true;
                btnDaftar.Text = "Daftar";
            }
        }
    }
}
