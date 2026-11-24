using System.Globalization;
using Aspose.Words;
using Microsoft.EntityFrameworkCore;
using MiniSoftware;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Enums;
using Squrl.App.Models;

namespace Squrl.App.Services.Report;

public sealed partial class ReportService
{
    public async Task<Result> GenerateItemPurchaseHistoryAsWordAsync(
        Guid itemId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        Item? existingItem = await _dataContext.Items
            .AsNoTracking()
            .Include(i => i.Uom)
            .Where(i => i.Id == itemId)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (existingItem == null)
        {
            return Result.Fail("Item not found.")
                .WithFailureType(FailureType.NotFound);
        }
        
        Document document;
        DocumentBuilder builder;

        // Initialize document.
        if (template != null)
        {
            document = new Document(template);

            Bookmark? bodyBookmark = document.Range.Bookmarks["BODY"];

            if (bodyBookmark == null)
            {
                return Result.Fail("Template must contain a BODY bookmark.")
                    .WithFailureType(FailureType.Validation);
            }

            builder = new(document);
            
            Bookmark? reportTitleeBookmark = document.Range.Bookmarks["REPORT_TITLE"];

            if (reportTitleeBookmark != null)
            {
                builder.MoveToBookmark("REPORT_TITLE");
                builder.Write("Item Purchase History");
            }
            
            Bookmark? customerBookmark = document.Range.Bookmarks["ITEM_NAME"];

            if (customerBookmark != null)
            {
                builder.MoveToBookmark("ITEM_NAME");
                builder.Write(existingItem.Name);
            }
            
            Bookmark? dateGeneratedBookmark = document.Range.Bookmarks["DATE_GENERATED"];

            if (dateGeneratedBookmark != null)
            {
                builder.MoveToBookmark("DATE_GENERATED");
                
                Instant generatedAt = _clock.GetCurrentInstant();
                
                ZonedDateTime timeZone = generatedAt.InZone(_timeZone);
                
                string formattedDate = timeZone.ToString(
                    "yyyy-MMM-dd HH:mm",
                    CultureInfo.InvariantCulture).ToUpperInvariant();

                builder.Write(formattedDate);
            }
            
            builder.MoveToBookmark("BODY");
        }
        else
        {
            document = new();
            builder = new(document);
            
            builder.Font.Bold = true;
            builder.Font.Italic = false;
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            builder.Write($"{existingItem.Name.ToUpperInvariant()} PURCHASE HISTORY");
            builder.Writeln();
            builder.Writeln();
            
            Instant generatedAt = _clock.GetCurrentInstant();
                
            ZonedDateTime timeZone = generatedAt.InZone(_timeZone);
                
            string formattedDate = timeZone.ToString(
                "yyyy-MMM-dd HH:mm",
                CultureInfo.InvariantCulture).ToUpperInvariant();

            builder.Font.Bold = false;
            builder.Font.Italic = true;
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            builder.Write($"Generated At: {formattedDate}");
            builder.Writeln();
            
            builder.Font.Bold = false;
            builder.Font.Italic = false;
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;
        }
        
        IAsyncEnumerable<PurchaseOrderDetail> existingPoDetails = _dataContext.PurchaseOrderDetails
            .AsNoTracking()
            .Include(p => p.PurchaseOrder)
            .ThenInclude(p => p.Supplier)
            .Where(p => p.Item.Id == itemId &&
                        p.PurchaseOrder.Status == PurchaseOrderStatus.Received)
            .OrderBy(p => p.PurchaseOrder.DateShipped)
            .ThenBy(s => EF.Property<Instant>(s, "DateCreated"))
            .AsAsyncEnumerable();
        
        builder.Font.Bold = true;
        
        builder.StartTable();

        builder.InsertCell();
        builder.Write("Supplier");

        builder.InsertCell();
        builder.Write("Quantity");
        
        builder.InsertCell();
        builder.Write("Unit Price");

        builder.InsertCell();
        builder.Write("Total");
        
        builder.InsertCell();
        builder.Write("Purchase Order ID");
        
        builder.InsertCell();
        builder.Write("Invoice No.");
        
        builder.InsertCell();
        builder.Write("Date Received");
        
        builder.Font.Bold = false;

        builder.EndRow();

        decimal totalQuantity = 0;
        decimal grandTotal = 0;
        
        // Populate table.
        await foreach (PurchaseOrderDetail poDetail
                       in existingPoDetails.WithCancellation(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            builder.InsertCell();
            builder.Write(poDetail.PurchaseOrder.Supplier?.Name ?? string.Empty);
            
            builder.InsertCell();
            builder.Write($"{poDetail.Quantity
                .ToString(CultureInfo.InvariantCulture)} {existingItem.Uom.Code}");
            
            totalQuantity +=  poDetail.Quantity;
            
            builder.InsertCell();
            builder.Write(_decimalFormatter.FormatCurrency(poDetail.UnitPrice));

            decimal total = poDetail.Quantity * poDetail.UnitPrice;
            
            builder.InsertCell();
            builder.Write(_decimalFormatter.FormatCurrency(total));
            
            grandTotal += total;
            
            builder.InsertCell();
            builder.Write(poDetail.PurchaseOrder.Id.ToString());
            
            builder.InsertCell();
            builder.Write(poDetail.PurchaseOrder.InvoiceNumber ?? string.Empty);
            
            builder.InsertCell();
            builder.Write(poDetail.PurchaseOrder.DateShipped?
                .InZone(_timeZone)
                .ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)
                .ToUpperInvariant() ?? string.Empty);

            builder.EndRow();
        }
        
        builder.InsertCell();
        builder.CellFormat.Borders.LineStyle = LineStyle.None;
        builder.Font.Bold = true;
        builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
        builder.Write("Grand Total:");

        builder.InsertCell();
        builder.CellFormat.Borders.LineStyle = LineStyle.Single;
        builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;
        builder.Write($"{totalQuantity
            .ToString(CultureInfo.InvariantCulture)} {existingItem.Uom.Code}");

        builder.InsertCell();
        builder.CellFormat.Borders.LineStyle = LineStyle.None;
        builder.Font.Bold = true;
        builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
        builder.Write("Grand Total:");

        builder.InsertCell();
        builder.CellFormat.Borders.LineStyle = LineStyle.Single;
        builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;
        builder.Write(_decimalFormatter.FormatCurrency(grandTotal));

        builder.Font.Bold = false;

        builder.EndRow();
        
        builder.EndTable();
        
        cancellationToken.ThrowIfCancellationRequested();
        
        document.Save(destination, SaveFormat.Docx);

        await destination.FlushAsync(cancellationToken);

        return Result.Ok();
    }

    public async Task<Result> GenerateItemPurchaseHistoryAsPdfAsync(
        Guid itemId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        // Generate Word document first.
        using MemoryStream wordStream = new();

        Result wordGeneration =
            await GenerateItemPurchaseHistoryAsWordAsync(
                itemId,
                wordStream,
                template,
                cancellationToken);

        if (!wordGeneration.IsSuccess)
        {
            return Result.Fail(wordGeneration.Message ?? "Something went wrong");
        }

        cancellationToken.ThrowIfCancellationRequested();
        
        wordStream.Position = 0;
        
        MiniPdf.ConvertToPdf(wordStream, destination);

        await destination.FlushAsync(cancellationToken);

        return Result.Ok();
    }
}