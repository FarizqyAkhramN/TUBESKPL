using PROJECTKPL.API.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PROJECTKPL.Tests
{
    [TestClass]
    public class PesananStateMachineTest
    {
        private Pesanan BuatPesanan()
        {
            var pelanggan = new Pelanggan
            {
                Id = 1,
                Username = "TestUser",
                Gender = "Laki-laki",
                NoTelp = "081234567890",
                Umur = 25,
                Password = "Test123"
            };

            var obat = new Obat("Paracetamol 500mg", 50, 5000);

            return new Pesanan
            {
                IdPesanan = "PSN001",
                Pelanggan = pelanggan,
                PelangganId = 1,
                Obat = obat,
                ObatId = 1,
                Jumlah = 2,
                MetodePengambilan = MetodePengambilan.Langsung,
                Pembayaran = 10000,
                Status = StatusPesanan.Keranjang
            };
        }

        [TestMethod]
        public void Konfirmasi_DariKeranjang_StatusJadiDiproses()
        {
            var pesanan = BuatPesanan();
            var result = pesanan.AktifkanTrigger(TriggerPesanan.Konfirmasi);
            Assert.AreEqual(StatusPesanan.Diproses, result);
            Assert.AreEqual(StatusPesanan.Diproses, pesanan.Status);
        }

        [TestMethod]
        public void Siapkan_DariDiproses_StatusJadiDisiapkan()
        {
            var pesanan = BuatPesanan();
            pesanan.AktifkanTrigger(TriggerPesanan.Konfirmasi);
            var result = pesanan.AktifkanTrigger(TriggerPesanan.Siapkan);
            Assert.AreEqual(StatusPesanan.Disiapkan, result);
            Assert.AreEqual(StatusPesanan.Disiapkan, pesanan.Status);
        }

        [TestMethod]
        public void AmbilDanBayar_DariDisiapkan_StatusJadiSelesai()
        {
            var pesanan = BuatPesanan();
            pesanan.AktifkanTrigger(TriggerPesanan.Konfirmasi);
            pesanan.AktifkanTrigger(TriggerPesanan.Siapkan);
            var result = pesanan.AktifkanTrigger(TriggerPesanan.AmbilDanBayar);
            Assert.AreEqual(StatusPesanan.Selesai, result);
            Assert.AreEqual(StatusPesanan.Selesai, pesanan.Status);
        }

        [TestMethod]
        public void Batalkan_DariKeranjang_StatusJadiDibatalkan()
        {
            var pesanan = BuatPesanan();
            var result = pesanan.AktifkanTrigger(TriggerPesanan.Batalkan);
            Assert.AreEqual(StatusPesanan.Dibatalkan, result);
            Assert.AreEqual(StatusPesanan.Dibatalkan, pesanan.Status);
        }

        [TestMethod]
        public void Batalkan_DariDiproses_StatusJadiDibatalkan()
        {
            var pesanan = BuatPesanan();
            pesanan.AktifkanTrigger(TriggerPesanan.Konfirmasi);
            var result = pesanan.AktifkanTrigger(TriggerPesanan.Batalkan);
            Assert.AreEqual(StatusPesanan.Dibatalkan, result);
            Assert.AreEqual(StatusPesanan.Dibatalkan, pesanan.Status);
        }

        [TestMethod]
        public void Siapkan_DariKeranjang_ReturnNull()
        {
            var pesanan = BuatPesanan();
            var result = pesanan.AktifkanTrigger(TriggerPesanan.Siapkan);
            Assert.IsNull(result);
            Assert.AreEqual(StatusPesanan.Keranjang, pesanan.Status);
        }

        [TestMethod]
        public void AmbilDanBayar_DariKeranjang_ReturnNull()
        {
            var pesanan = BuatPesanan();
            var result = pesanan.AktifkanTrigger(TriggerPesanan.AmbilDanBayar);
            Assert.IsNull(result);
            Assert.AreEqual(StatusPesanan.Keranjang, pesanan.Status);
        }

        [TestMethod]
        public void Konfirmasi_DariSelesai_ReturnNull()
        {
            var pesanan = BuatPesanan();
            pesanan.AktifkanTrigger(TriggerPesanan.Konfirmasi);
            pesanan.AktifkanTrigger(TriggerPesanan.Siapkan);
            pesanan.AktifkanTrigger(TriggerPesanan.AmbilDanBayar);
            var result = pesanan.AktifkanTrigger(TriggerPesanan.Konfirmasi);
            Assert.IsNull(result);
            Assert.AreEqual(StatusPesanan.Selesai, pesanan.Status);
        }

        [TestMethod]
        public void Batalkan_DariSelesai_ReturnNull()
        {
            var pesanan = BuatPesanan();
            pesanan.AktifkanTrigger(TriggerPesanan.Konfirmasi);
            pesanan.AktifkanTrigger(TriggerPesanan.Siapkan);
            pesanan.AktifkanTrigger(TriggerPesanan.AmbilDanBayar);
            var result = pesanan.AktifkanTrigger(TriggerPesanan.Batalkan);
            Assert.IsNull(result);
            Assert.AreEqual(StatusPesanan.Selesai, pesanan.Status);
        }
    }
}
