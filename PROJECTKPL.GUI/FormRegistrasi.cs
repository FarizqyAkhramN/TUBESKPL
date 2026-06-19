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
            Text = "Daftar Akun Baru — ApoTek";
            Size = new Size(400, 500);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9f);

            var pnlCard = new Panel
            {
                Size = new Size(340, 420),
                Location = new Point(30, 20),
                BackColor = Color.White
            };

            lblJudul = new Label
            {
                Text = "Daftar Akun Baru",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 20),
                AutoSize = true
            };

            int top = 60;

            var lblUsr = BuatLabel("Username", top);
            txtUsername = BuatTextBox(top + 20);
            top += 60;

            var lblGen = BuatLabel("Gender", top);
            cmbGender = new ComboBox
            {
                Location = new Point(20, top + 20),
                Size = new Size(300, 26),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9f)
            };
            cmbGender.Items.AddRange(new object[] { "Laki-laki", "Perempuan" });
            cmbGender.SelectedIndex = 0;
            top += 60;

            var lblTlp = BuatLabel("No. Telepon (awali 08)", top);
            txtNoTelp = BuatTextBox(top + 20);
            top += 60;

            var lblUmr = BuatLabel("Umur", top);
            txtUmur = BuatTextBox(top + 20);
            top += 60;

            var lblPass = BuatLabel("Password", top);
            txtPassword = BuatTextBox(top + 20);
            txtPassword.PasswordChar = '●';
            top += 60;

            var lblKonfirm = BuatLabel("Konfirmasi Password", top);
            txtKonfirmPassword = BuatTextBox(top + 20);
            txtKonfirmPassword.PasswordChar = '●';
            top += 60;

            lblStatus = new Label
            {
                Text = "",
                Location = new Point(20, top),
                Size = new Size(300, 20),
                Font = new Font("Segoe UI", 8.5f),
                AutoSize = false
            };
            top += 25;

            btnDaftar = new Button
            {
                Text = "Daftar",
                Location = new Point(20, top),
                Size = new Size(140, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDaftar.FlatAppearance.BorderSize = 0;
            btnDaftar.Click += BtnDaftar_Click;

            btnBatal = new Button
            {
                Text = "Batal",
                Location = new Point(170, top),
                Size = new Size(140, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(226, 232, 240),
                ForeColor = Color.FromArgb(51, 65, 85),
                Font = new Font("Segoe UI", 9f),
                Cursor = Cursors.Hand
            };
            btnBatal.FlatAppearance.BorderSize = 0;
            btnBatal.Click += (s, e) => Close();

            pnlCard.Controls.AddRange(new Control[]
            {
                lblJudul,
                lblUsr, txtUsername,
                lblGen, cmbGender,
                lblTlp, txtNoTelp,
                lblUmr, txtUmur,
                lblPass, txtPassword,
                lblKonfirm, txtKonfirmPassword,
                lblStatus, btnDaftar, btnBatal
            });

            Controls.Add(pnlCard);
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
