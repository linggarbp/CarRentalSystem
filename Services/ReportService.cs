using CarRentalSystem.Models;
using CarRentalSystem.Services;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CarRentalSystem.Services
{
    public class ReportService : IReportService
    {
        public async Task<byte[]> GenerateRentalsExcelReportAsync(IEnumerable<Rental> rentals)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Rental Report");

            // Headers
            string[] headers = {
        "Rental ID", "User Name", "User Email", "Car Brand", "Car Model",
        "License Plate", "Start Date", "End Date", "Total Days", "Total Price",
        "Status", "Created Date"
    };

            // Add headers
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }

            // Style headers without background fill
            var headerRange = worksheet.Cells[1, 1, 1, headers.Length];
            headerRange.Style.Font.Bold = true;
            headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            // Add borders instead of background color
            headerRange.Style.Border.Top.Style = ExcelBorderStyle.Thick;
            headerRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            headerRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            headerRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            // Data rows
            int row = 2;
            foreach (var rental in rentals)
            {
                worksheet.Cells[row, 1].Value = rental.Id;
                worksheet.Cells[row, 2].Value = rental.User?.FullName ?? "N/A";
                worksheet.Cells[row, 3].Value = rental.User?.Email ?? "N/A";
                worksheet.Cells[row, 4].Value = rental.Car?.Brand ?? "N/A";
                worksheet.Cells[row, 5].Value = rental.Car?.Model ?? "N/A";
                worksheet.Cells[row, 6].Value = rental.Car?.LicensePlate ?? "N/A";
                worksheet.Cells[row, 7].Value = rental.StartDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 8].Value = rental.EndDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 9].Value = rental.TotalDays;
                worksheet.Cells[row, 10].Value = rental.TotalPrice;
                worksheet.Cells[row, 11].Value = rental.Status.ToString();
                worksheet.Cells[row, 12].Value = rental.CreatedDate.ToString("yyyy-MM-dd HH:mm");

                worksheet.Cells[row, 10].Style.Numberformat.Format = "Rp #,##0";

                // Add row borders
                var rowRange = worksheet.Cells[row, 1, row, headers.Length];
                rowRange.Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

                row++;
            }

            // Summary
            var summaryRow = row + 1;
            worksheet.Cells[summaryRow, 9].Value = "TOTAL:";
            worksheet.Cells[summaryRow, 9].Style.Font.Bold = true;
            worksheet.Cells[summaryRow, 10].Formula = $"SUM(J2:J{row - 1})";
            worksheet.Cells[summaryRow, 10].Style.Font.Bold = true;
            worksheet.Cells[summaryRow, 10].Style.Numberformat.Format = "Rp #,##0";

            // Summary borders
            var summaryRange = worksheet.Cells[summaryRow, 9, summaryRow, 10];
            summaryRange.Style.Border.Top.Style = ExcelBorderStyle.Thick;
            summaryRange.Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // Set column widths for better appearance
            worksheet.Column(1).Width = 10;  // ID
            worksheet.Column(2).Width = 20;  // User Name
            worksheet.Column(3).Width = 25;  // Email
            worksheet.Column(4).Width = 15;  // Brand
            worksheet.Column(5).Width = 15;  // Model
            worksheet.Column(6).Width = 15;  // License
            worksheet.Column(7).Width = 12;  // Start Date
            worksheet.Column(8).Width = 12;  // End Date
            worksheet.Column(9).Width = 10;  // Days
            worksheet.Column(10).Width = 12; // Price
            worksheet.Column(11).Width = 12; // Status
            worksheet.Column(12).Width = 18; // Created Date

            return await Task.FromResult(package.GetAsByteArray());
        }


        public async Task<byte[]> GenerateRentalsPdfReportAsync(IEnumerable<Rental> rentals)
        {
            try
            {
                using var stream = new MemoryStream();
                using var writer = new PdfWriter(stream);
                using var pdf = new PdfDocument(writer);
                using var document = new Document(pdf, PageSize.A4.Rotate()); // Landscape for more columns

                document.Add(new Paragraph("Car Rental Report")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(18)
                    .SetMarginBottom(10));

                document.Add(new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm}")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(10)
                    .SetMarginBottom(20));

                var table = new Table(7).UseAllAvailableWidth(); // 7 columns

                table.AddHeaderCell(new Cell().Add(new Paragraph("ID")).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER));
                table.AddHeaderCell(new Cell().Add(new Paragraph("User")).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Car")).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Start Date")).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER));
                table.AddHeaderCell(new Cell().Add(new Paragraph("End Date")).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Total Price")).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Status")).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.CENTER));

                // Data rows
                decimal totalRevenue = 0;
                int totalRentals = 0;

                foreach (var rental in rentals)
                {
                    table.AddCell(new Cell().Add(new Paragraph(rental.Id.ToString())).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(rental.User?.FullName ?? "N/A")).SetTextAlignment(TextAlignment.LEFT));
                    table.AddCell(new Cell().Add(new Paragraph($"{rental.Car?.Brand ?? "N/A"} {rental.Car?.Model ?? "N/A"}")).SetTextAlignment(TextAlignment.LEFT));
                    table.AddCell(new Cell().Add(new Paragraph(rental.StartDate.ToString("MM/dd/yyyy"))).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(rental.EndDate.ToString("MM/dd/yyyy"))).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph($"${rental.TotalPrice:F2}")).SetTextAlignment(TextAlignment.RIGHT));

                    var statusCell = new Cell().Add(new Paragraph(rental.Status.ToString())).SetTextAlignment(TextAlignment.CENTER);
                    if (rental.Status == RentalStatus.Active)
                    {
                        statusCell.SetBackgroundColor(ColorConstants.YELLOW);
                    }
                    else
                    {
                        statusCell.SetBackgroundColor(ColorConstants.GREEN);
                    }
                    table.AddCell(statusCell);

                    totalRevenue += rental.TotalPrice;
                    totalRentals++;
                }

                document.Add(table);

                // Summary
                document.Add(new Paragraph($"\nSummary: {totalRentals} total rentals")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontSize(12)
                    .SetMarginTop(20));

                document.Add(new Paragraph($"Total Revenue: ${totalRevenue:F2}")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontSize(14)
                    .SetMarginTop(5));

                document.Close();
                return await Task.FromResult(stream.ToArray());
            }
            catch (Exception ex)
            {
                // More detailed error logging
                Console.WriteLine($"PDF Error Details: {ex}");
                throw new Exception($"PDF generation failed: {ex.Message}");
            }
        }
    }
}
