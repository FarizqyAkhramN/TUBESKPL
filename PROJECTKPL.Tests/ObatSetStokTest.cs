using PROJECTKPL.API.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PROJECTKPL.Tests
{
    [TestClass]
    public class ObatSetStokTest
    {
        // ── Status berdasarkan stok ───────────────────────────────────────
        [TestMethod]
        public void SetStok_Nol_StatusJadiHabis()
        {
            var obat = new Obat("Paracetamol", 50, 5000);
            obat.SetStok(0);
            Assert.AreEqual(StatusObat.Habis, obat.StatusObat);
            Assert.AreEqual(0, obat.Stok);
        }

        [TestMethod]
        public void SetStok_SamadenganSepuluh_StatusJadiHampirHabis()
        {
            var obat = new Obat("Paracetamol", 50, 5000);
            obat.SetStok(10);
            Assert.AreEqual(StatusObat.HampirHabis, obat.StatusObat);
            Assert.AreEqual(10, obat.Stok);
        }

        [TestMethod]
        public void SetStok_KurangDariSepuluh_StatusJadiHampirHabis()
        {
            var obat = new Obat("Paracetamol", 50, 5000);
            obat.SetStok(5);
            Assert.AreEqual(StatusObat.HampirHabis, obat.StatusObat);
            Assert.AreEqual(5, obat.Stok);
        }

        [TestMethod]
        public void SetStok_LebihDariSepuluh_StatusJadiTersedia()
        {
            var obat = new Obat("Paracetamol", 0, 5000);
            obat.SetStok(20);
            Assert.AreEqual(StatusObat.Tersedia, obat.StatusObat);
            Assert.AreEqual(20, obat.Stok);
        }

        [TestMethod]
        public void SetStok_SatuSatuSatu_StatusJadiTersedia()
        {
            var obat = new Obat("Paracetamol", 0, 5000);
            obat.SetStok(111);
            Assert.AreEqual(StatusObat.Tersedia, obat.StatusObat);
        }

        // ── Status awal saat konstruksi ───────────────────────────────────
        [TestMethod]
        public void Constructor_StokNol_StatusLangsungHabis()
        {
            var obat = new Obat("Antasida", 0, 3000);
            Assert.AreEqual(StatusObat.Habis, obat.StatusObat);
        }

        [TestMethod]
        public void Constructor_StokDelapan_StatusLangsungHampirHabis()
        {
            var obat = new Obat("Amoxicillin", 8, 12000);
            Assert.AreEqual(StatusObat.HampirHabis, obat.StatusObat);
        }

        [TestMethod]
        public void Constructor_StokLimaPuluh_StatusLangsungTersedia()
        {
            var obat = new Obat("Paracetamol", 50, 5000);
            Assert.AreEqual(StatusObat.Tersedia, obat.StatusObat);
        }

        // ── Perubahan status dinamis ──────────────────────────────────────
        [TestMethod]
        public void SetStok_DariTersediaKeHabis_StatusBerubah()
        {
            var obat = new Obat("Paracetamol", 50, 5000);
            Assert.AreEqual(StatusObat.Tersedia, obat.StatusObat);
            obat.SetStok(0);
            Assert.AreEqual(StatusObat.Habis, obat.StatusObat);
        }

        [TestMethod]
        public void SetStok_DariHabisKeTersedia_StatusBerubah()
        {
            var obat = new Obat("Paracetamol", 0, 5000);
            Assert.AreEqual(StatusObat.Habis, obat.StatusObat);
            obat.SetStok(50);
            Assert.AreEqual(StatusObat.Tersedia, obat.StatusObat);
        }
    }
}
