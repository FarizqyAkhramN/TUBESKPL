using PROJECTKPL.API.Models;
using PROJECTKPL.API.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace PROJECTKPL.Tests
{
    [TestClass]
    public class PelangganValidatorTest
    {
        private readonly PelangganValidator _validator = new();

        private Pelanggan PelangganValid() => new Pelanggan
        {
            Username = "Budi123",
            Gender = "Laki-laki",
            NoTelp = "081234567890",
            Umur = 25,
            Password = "Budi123"
        };

        [TestMethod]
        public void Username_Valid_LolosValidasi()
        {
            var p = PelangganValid();
            var result = _validator.Validate(p, "Username");
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Username_Kosong_GagalValidasi()
        {
            var p = PelangganValid();
            p.Username = "";
            var result = _validator.Validate(p, "Username");
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Username_TerlalupPendek_GagalValidasi()
        {
            var p = PelangganValid();
            p.Username = "ab";
            var result = _validator.Validate(p, "Username");
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Gender_LakiLaki_LolosValidasi()
        {
            var p = PelangganValid();
            p.Gender = "Laki-laki";
            var result = _validator.Validate(p, "Gender");
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Gender_Perempuan_LolosValidasi()
        {
            var p = PelangganValid();
            p.Gender = "Perempuan";
            var result = _validator.Validate(p, "Gender");
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Gender_HurufKecil_GagalValidasi()
        {
            var p = PelangganValid();
            p.Gender = "laki-laki";
            var result = _validator.Validate(p, "Gender");
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Gender_NilaiLain_GagalValidasi()
        {
            var p = PelangganValid();
            p.Gender = "Unknown";
            var result = _validator.Validate(p, "Gender");
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void NoTelp_Valid_LolosValidasi()
        {
            var p = PelangganValid();
            p.NoTelp = "081234567890";
            var result = _validator.Validate(p, "NoTelp");
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void NoTelp_TidakDiawali08_GagalValidasi()
        {
            var p = PelangganValid();
            p.NoTelp = "091234567890";
            var result = _validator.Validate(p, "NoTelp");
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void NoTelp_TerlalupPendek_GagalValidasi()
        {
            var p = PelangganValid();
            p.NoTelp = "081";
            var result = _validator.Validate(p, "NoTelp");
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Password_Valid_LolosValidasi()
        {
            var p = PelangganValid();
            p.Password = "Budi123";
            var result = _validator.Validate(p, "Password");
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Password_TerlalupPendek_GagalValidasi()
        {
            var p = PelangganValid();
            p.Password = "Ab1";
            var result = _validator.Validate(p, "Password");
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Password_TanpaHurufKapital_GagalValidasi()
        {
            var p = PelangganValid();
            p.Password = "budi123";
            var result = _validator.Validate(p, "Password");
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Password_TanpaAngka_GagalValidasi()
        {
            var p = PelangganValid();
            p.Password = "BudiAbc";
            var result = _validator.Validate(p, "Password");
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void ValidateAll_DataValid_LolosSemuaRuleset()
        {
            var p = PelangganValid();
            var result = _validator.ValidateAll(p);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void ValidateAll_DataTidakValid_GagalDenganPesanError()
        {
            var p = new Pelanggan
            {
                Username = "",
                Gender = "laki-laki",
                NoTelp = "081",
                Umur = 25,
                Password = "123"
            };
            var result = _validator.ValidateAll(p);
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count > 0);
        }
    }
}
