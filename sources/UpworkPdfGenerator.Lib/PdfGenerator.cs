using System;
using System.Globalization;
using System.IO;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using UpworkPdfGenerator.Lib.Utilities;

#nullable enable

namespace UpworkPdfGenerator.Lib
{
    public class PdfGenerator
    {
        public static void GenerateConfirmationOfServicesForm(
            Stream destinationStream,
            byte[] signBytes,
            string contractorRus,
            string contractorEnd,
            double value,
            DateTime? date = null)
        {
            date ??= DateTime.UtcNow;

            using var sourceStream = ResourcesUtilities.ReadFileAsStream("Confirmation of Services Form.pdf");
            using var document = new PdfDocument(
                new PdfReader(sourceStream), 
                new PdfWriter(destinationStream));
            var fontBytes = ResourcesUtilities.ReadFileAsBytes("Times New Roman Cyrillic.ttf");
            var font = PdfFontFactory.CreateFont(fontBytes, "Cp1251", true);

            {
                var canvas = new PdfCanvas(document.GetFirstPage());
                canvas
                    .BeginText()
                    .SetFontAndSize(font, 11)
                    .MoveText(100, 657)
                    .ShowText($"{date.Value.ToString("dd MMMM yyyy", CultureInfo.GetCultureInfo("ru-RU"))}")
                    .MoveText(-30, -100)
                    .ShowText(contractorRus)
                    .MoveText(314, -233)
                    .ShowText($"{value:N}")
                    .MoveText(-317, -128)
                    .ShowText(contractorRus)
                    .EndText();

                canvas
                    .AddImage(
                        ImageDataFactory.CreatePng(signBytes), 
                        new Rectangle(65, 210, 50, 50), 
                        true);
            }

            {
                var canvas = new PdfCanvas(document.GetLastPage());
                canvas
                    .BeginText()
                    .SetFontAndSize(font, 11)
                    .MoveText(100, 662)
                    .ShowText(date.Value.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture))
                    .MoveText(-30, -90)
                    .ShowText(contractorEnd)
                    .MoveText(266, -184)
                    .ShowText(value.ToString("N", CultureInfo.InvariantCulture))
                    .MoveText(-270, -175)
                    .ShowText(contractorEnd)
                    .EndText();

                canvas
                    .AddImage(
                        ImageDataFactory.CreatePng(signBytes),
                        new Rectangle(65, 225, 50, 50),
                        true);
            }
        }
    }
}
