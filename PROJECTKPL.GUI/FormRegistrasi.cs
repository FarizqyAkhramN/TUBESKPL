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
                Location = new Point(left, top + 20),
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
            // Validasi input
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtNoTelp.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Semua field harus diisi.";
                return;
            }

            if (txtPassword.Text != txtKonfirmPassword.Text)
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Password tidak cocok.";
                return;
            }

            if (!int.TryParse(txtUmur.Text, out int umur) || umur < 1 || umur > 120)
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Umur tidak valid.";
                return;
            }

            btnDaftar.Enabled = false;
            try
            {
                var res = await _http.PostAsJsonAsync("api/pelanggan", new
                {
                    username = txtUsername.Text.Trim(),
                    gender = cmbGender.SelectedItem?.ToString() ?? "",
                    noTelp = txtNoTelp.Text.Trim(),
                    umur,
                    password = txtPassword.Text
                });

                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Akun berhasil dibuat! Silakan login.",
                        "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
                else
                {
                    lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                    lblStatus.Text = "Pendaftaran gagal. Cek kembali data.";
                }
            }
            catch
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "Tidak dapat terhubung ke server.";
            }
            finally
            {
                btnDaftar.Enabled = true;
            }
        }
    }
}
