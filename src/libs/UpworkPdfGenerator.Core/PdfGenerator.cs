﻿using System.Globalization;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

namespace UpworkPdfGenerator.Core;

public class PdfGenerator
{
    public static void GenerateConfirmationOfServicesForm(
        Stream destinationStream,
        byte[] signBytes,
        string contractorRus,
        string contractorEnd,
        double amount,
        DateTime? date = null)
    {
        date ??= DateTime.UtcNow;

        using var sourceStream = H.Resources.Confirmation_of_Services_Form_pdf.AsStream();
        using var document = new PdfDocument(
            new PdfReader(sourceStream),
            new PdfWriter(destinationStream));
        var fontBytes = H.Resources.Times_New_Roman_Cyrillic_ttf.AsBytes();
        var font = PdfFontFactory.CreateFont(fontBytes, "Cp1251", PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);

        {
            var canvas = new PdfCanvas(document.GetFirstPage());
            canvas
                .BeginText()
                .SetFontAndSize(font, 11)
                .MoveText(100, 659)
                .ShowText($"{date.Value.ToString("dd MMMM yyyy", CultureInfo.GetCultureInfo("ru-RU"))}")
                .MoveText(-30, -100)
                .ShowText(contractorRus)
                .MoveText(316, -222)
                .ShowText($"{amount.ToString("F2", CultureInfo.InvariantCulture)}")
                .MoveText(-317, -140)
                .ShowText(contractorRus)
                .EndText();

            if (signBytes.Any())
            {
                canvas
                    .AddImageFittedIntoRectangle(
                        ImageDataFactory.CreatePng(signBytes),
                        new Rectangle(65, 210, 50, 50),
                        true);
            }
        }

        {
            var canvas = new PdfCanvas(document.GetLastPage());
            canvas
                .BeginText()
                .SetFontAndSize(font, 11)
                .MoveText(100, 672)
                .ShowText(date.Value.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture))
                .MoveText(-30, -90)
                .ShowText(contractorEnd)
                .MoveText(270, -177)
                .ShowText(amount.ToString("F2", CultureInfo.InvariantCulture))
                .MoveText(-270, -185)
                .ShowText(contractorEnd)
                .EndText();

            if (signBytes.Any())
            {
                canvas
                    .AddImageFittedIntoRectangle(
                        ImageDataFactory.CreatePng(signBytes),
                        new Rectangle(65, 245, 50, 50),
                        true);
            }
        }
    }
}
