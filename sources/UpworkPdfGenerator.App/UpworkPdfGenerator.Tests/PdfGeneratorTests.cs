using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpworkPdfGenerator.Lib;

namespace UpworkPdfGenerator.Tests
{
    [TestClass]
    public class PdfGeneratorTests
    {
        [TestMethod]
        public void GenerateTest()
        {
            using var destinationStream = new MemoryStream();
            var signPngBytes = File.ReadAllBytes(@"D:\Backup\My\sign.png");

            var date = DateTime.UtcNow;
            PdfGenerator.GenerateConfirmationOfServicesForm(
                destinationStream,
                signPngBytes,
                "�� ������ ���������� ����������",
                "Stukov Konstantin Mihaylovich (Individual entrepreneur)",
                1757.44,
                date);

            var bytes = destinationStream.ToArray();
            var path = Path.Combine(
                Path.GetTempPath(), 
                $"Confirmation of Services Form - {date.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture)}.pdf");

            File.WriteAllBytes(path, bytes);

            Process.Start(
                $"{Environment.GetEnvironmentVariable("LOCALAPPDATA")}\\Google\\Chrome\\Application\\chrome.exe", 
                $"\"{path}\"");
        }
    }
}
