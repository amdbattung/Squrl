using System.Globalization;
using Aspose.Words;
using Microsoft.EntityFrameworkCore;
using MiniSoftware;
using NodaTime;
using Squrl.App.Common;
using Squrl.App.Data;
using Squrl.App.Enums;
using Squrl.App.Models;

namespace Squrl.App.Services.Report;

public class ReportService : IReportService
{
    private readonly DataContext _dataContext;
    private readonly IClock _clock;

    public ReportService(DataContext dataContext,
        IClock clock)
    {
        _dataContext = dataContext;
        _clock = clock;
    }
    
    
    public async Task<Result<bool>> GenerateCustomerItemSalesAsWord(
        string customer,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        Document document;
        DocumentBuilder builder;

        // Initialize document.
        if (template != null)
        {
            document = new Document(template);

            Bookmark? bodyBookmark = document.Range.Bookmarks["BODY"];

            if (bodyBookmark == null)
            {
                return Result<bool>.Fail("Template must contain a BODY bookmark.")
                    .WithFailureType(FailureType.Validation);
            }

            builder = new DocumentBuilder(document);
            
            Bookmark? reportTitleeBookmark = document.Range.Bookmarks["REPORT_TITLE"];

            if (reportTitleeBookmark != null)
            {
                builder.MoveToBookmark("REPORT_TITLE");
                builder.Write("Customer Item Sales Summary");
            }
            
            Bookmark? customerBookmark = document.Range.Bookmarks["CUSTOMER"];

            if (customerBookmark != null)
            {
                builder.MoveToBookmark("CUSTOMER");
                builder.Write(customer);
            }
            
            Bookmark? dateGeneratedBookmark = document.Range.Bookmarks["DATE_GENERATED"];

            if (dateGeneratedBookmark != null)
            {
                builder.MoveToBookmark("DATE_GENERATED");
                
                Instant generatedAt = _clock.GetCurrentInstant();
                
                ZonedDateTime timeZone = generatedAt.InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault());
                
                string formattedDate = timeZone.ToString(
                    "yyyy-MMM-dd HH:mm",
                    CultureInfo.InvariantCulture).ToUpperInvariant();

                builder.Write(formattedDate);
            }
            
            builder.MoveToBookmark("BODY");
        }
        else
        {
            document = new Document();
            builder = new DocumentBuilder(document);
            
            builder.Font.Bold = true;
            builder.Font.Italic = false;
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            builder.Write($"ITEM SALES OF {customer.ToUpperInvariant()}");
            builder.Writeln();
            builder.Writeln();
            
            Instant generatedAt = _clock.GetCurrentInstant();
                
            ZonedDateTime timeZone = generatedAt.InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault());
                
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
        
        Dictionary<Guid, ItemTransactionSummary> itemSales = new();
        
        IAsyncEnumerable<SalesOrderDetail> existingSoDetails = _dataContext.SalesOrderDetails
            .AsNoTracking()
            .Include(s => s.Item)
            .ThenInclude(i => i.Uom)
            .Where(s =>
                s.SalesOrder.Customer != null &&
                s.SalesOrder.Customer.ToLower() == customer.ToLower())
            .OrderBy(s => s.Item.Name)
            .ThenBy(s => s.Item.Id)
            .AsAsyncEnumerable();

        // Stream and aggregate database records.
        await foreach (SalesOrderDetail soDetail in existingSoDetails.WithCancellation(cancellationToken))
        {
            Guid itemId = soDetail.Item.Id;

            if (itemSales.TryGetValue(itemId, out ItemTransactionSummary? current))
            {
                itemSales[itemId] = current with
                {
                    TotalQuantity = current.TotalQuantity + soDetail.Quantity,

                    TotalPrice = current.TotalPrice + (soDetail.UnitPrice * soDetail.Quantity)
                };
            }
            else
            {
                itemSales[itemId] = new ItemTransactionSummary(
                    Name: soDetail.Item.Name,
                    Description: soDetail.Item.Description ?? string.Empty,
                    Uom: soDetail.Item.Uom.Code,
                    TotalQuantity: soDetail.Quantity,
                    TotalPrice: soDetail.UnitPrice * soDetail.Quantity
                );
            }
        }
        
        builder.Font.Bold = true;
        
        builder.StartTable();

        builder.InsertCell();
        builder.Write("Item");

        builder.InsertCell();
        builder.Write("Description");

        builder.InsertCell();
        builder.Write("Quantity Sold");

        builder.InsertCell();
        builder.Write("Total Sales");
        
        builder.Font.Bold = false;

        builder.EndRow();

        // Populate table.
        foreach (KeyValuePair<Guid, ItemTransactionSummary> item in
                 itemSales.OrderBy(s => s.Value.Name))
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            builder.InsertCell();
            builder.Write(item.Value.Name);
            
            builder.InsertCell();
            builder.Write(item.Value.Description);
            
            builder.InsertCell();
            builder.Write($"{item.Value.TotalQuantity
                .ToString(CultureInfo.InvariantCulture)} {item.Value.Uom}");
            
            builder.InsertCell();
            builder.Write(item.Value.TotalPrice
                .ToString(CultureInfo.InvariantCulture));

            builder.EndRow();
        }
        
        decimal grossTotal = itemSales.Values.Sum(s => s.TotalPrice);

        builder.InsertCell();
        builder.CellFormat.Borders.LineStyle = LineStyle.None;
        builder.Write(string.Empty);
        
        builder.InsertCell();
        builder.CellFormat.Borders.LineStyle = LineStyle.None;
        builder.Write(string.Empty);

        builder.InsertCell();
        builder.CellFormat.Borders.LineStyle = LineStyle.None;
        builder.Font.Bold = true;
        builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
        builder.Write("Grand Total:");

        builder.InsertCell();
        builder.CellFormat.Borders.LineStyle = LineStyle.Single;
        builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;
        builder.Write(grossTotal.ToString(CultureInfo.InvariantCulture));

        builder.Font.Bold = false;

        builder.EndRow();
        
        builder.EndTable();

        cancellationToken.ThrowIfCancellationRequested();
        
        document.Save(destination, SaveFormat.Docx);

        await destination.FlushAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }
    
    public async Task<Result<bool>> GenerateCustomerItemSalesAsPdf(
        string customer,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        // Generate Word document first.
        using MemoryStream wordStream = new MemoryStream();

        Result<bool> wordGeneration =
            await GenerateCustomerItemSalesAsWord(
                customer,
                wordStream,
                template,
                cancellationToken);

        if (!wordGeneration.IsSuccess)
        {
            return Result<bool>.Fail(wordGeneration.Message ?? "Something went wrong");
        }

        cancellationToken.ThrowIfCancellationRequested();
        
        wordStream.Position = 0;
        
        MiniPdf.ConvertToPdf(wordStream, destination);

        await destination.FlushAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> GenerateSupplierItemPurchasesAsWord(
        Guid supplierId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        string? existingSupplierName = await _dataContext.Suppliers
            .AsNoTracking()
            .Where(s => s.Id == supplierId)
            .Select(s => s.Name)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (existingSupplierName == null)
        {
            return Result<bool>.Fail("Supplier not found.")
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
                return Result<bool>.Fail("Template must contain a BODY bookmark.")
                    .WithFailureType(FailureType.Validation);
            }

            builder = new DocumentBuilder(document);
            
            Bookmark? reportTitleeBookmark = document.Range.Bookmarks["REPORT_TITLE"];

            if (reportTitleeBookmark != null)
            {
                builder.MoveToBookmark("REPORT_TITLE");
                builder.Write("Supplier Item Purchases Summary");
            }
            
            Bookmark? customerBookmark = document.Range.Bookmarks["SUPPLIER"];

            if (customerBookmark != null)
            {
                builder.MoveToBookmark("SUPPLIER");
                builder.Write(existingSupplierName);
            }
            
            Bookmark? dateGeneratedBookmark = document.Range.Bookmarks["DATE_GENERATED"];

            if (dateGeneratedBookmark != null)
            {
                builder.MoveToBookmark("DATE_GENERATED");
                
                Instant generatedAt = _clock.GetCurrentInstant();
                
                ZonedDateTime timeZone = generatedAt.InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault());
                
                string formattedDate = timeZone.ToString(
                    "yyyy-MMM-dd HH:mm",
                    CultureInfo.InvariantCulture).ToUpperInvariant();

                builder.Write(formattedDate);
            }
            
            builder.MoveToBookmark("BODY");
        }
        else
        {
            document = new Document();
            builder = new DocumentBuilder(document);
            
            builder.Font.Bold = true;
            builder.Font.Italic = false;
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            builder.Write($"ITEM PURCHASES FROM {existingSupplierName.ToUpperInvariant()}");
            builder.Writeln();
            builder.Writeln();
            
            Instant generatedAt = _clock.GetCurrentInstant();
                
            ZonedDateTime timeZone = generatedAt.InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault());
                
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
        
        Dictionary<Guid, ItemTransactionSummary> itemSales = new();
        
        IAsyncEnumerable<PurchaseOrderDetail> existingPoDetails = _dataContext.PurchaseOrderDetails
            .AsNoTracking()
            .Include(p => p.Item)
            .ThenInclude(p => p.Uom)
            .Where(p => p.PurchaseOrder.Supplier != null &&
                        p.PurchaseOrder.Supplier.Id == supplierId &&
                        p.PurchaseOrder.Status == PurchaseOrderStatus.Received)
            .OrderBy(p => p.Item.Name)
            .ThenBy(p => p.Item.Id)
            .AsAsyncEnumerable();

        // Stream and aggregate database records.
        await foreach (PurchaseOrderDetail poDetail in existingPoDetails.WithCancellation(cancellationToken))
        {
            Guid itemId = poDetail.Item.Id;

            if (itemSales.TryGetValue(itemId, out ItemTransactionSummary? current))
            {
                itemSales[itemId] = current with
                {
                    TotalQuantity = current.TotalQuantity + poDetail.Quantity,

                    TotalPrice = current.TotalPrice + (poDetail.UnitPrice * poDetail.Quantity)
                };
            }
            else
            {
                itemSales[itemId] = new ItemTransactionSummary(
                    Name: poDetail.Item.Name,
                    Description: poDetail.Item.Description ?? string.Empty,
                    Uom: poDetail.Item.Uom.Code,
                    TotalQuantity: poDetail.Quantity,
                    TotalPrice: poDetail.UnitPrice * poDetail.Quantity
                );
            }
        }
        
        builder.Font.Bold = true;
        
        builder.StartTable();

        builder.InsertCell();
        builder.Write("Item");

        builder.InsertCell();
        builder.Write("Description");

        builder.InsertCell();
        builder.Write("Quantity Purchased");

        builder.InsertCell();
        builder.Write("Total Cost");
        
        builder.Font.Bold = false;

        builder.EndRow();

        // Populate table.
        foreach (KeyValuePair<Guid, ItemTransactionSummary> item in
                 itemSales.OrderBy(s => s.Value.Name))
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            builder.InsertCell();
            builder.Write(item.Value.Name);
            
            builder.InsertCell();
            builder.Write(item.Value.Description);
            
            builder.InsertCell();
            builder.Write($"{item.Value.TotalQuantity
                .ToString(CultureInfo.InvariantCulture)} {item.Value.Uom}");
            
            builder.InsertCell();
            builder.Write(item.Value.TotalPrice
                .ToString(CultureInfo.InvariantCulture));

            builder.EndRow();
        }
        
        decimal grossTotal = itemSales.Values.Sum(s => s.TotalPrice);

        builder.InsertCell();
        builder.CellFormat.Borders.LineStyle = LineStyle.None;
        builder.Write(string.Empty);
        
        builder.InsertCell();
        builder.CellFormat.Borders.LineStyle = LineStyle.None;
        builder.Write(string.Empty);

        builder.InsertCell();
        builder.CellFormat.Borders.LineStyle = LineStyle.None;
        builder.Font.Bold = true;
        builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
        builder.Write("Grand Total:");

        builder.InsertCell();
        builder.CellFormat.Borders.LineStyle = LineStyle.Single;
        builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;
        builder.Write(grossTotal.ToString(CultureInfo.InvariantCulture));

        builder.Font.Bold = false;

        builder.EndRow();
        
        builder.EndTable();

        cancellationToken.ThrowIfCancellationRequested();
        
        document.Save(destination, SaveFormat.Docx);

        await destination.FlushAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }

    public async Task<Result<bool>> GenerateSupplierItemPurchasesAsPdf(
        Guid supplierId,
        Stream destination,
        Stream? template = null,
        CancellationToken cancellationToken = default)
    {
        // Generate Word document first.
        using MemoryStream wordStream = new MemoryStream();

        Result<bool> wordGeneration =
            await GenerateSupplierItemPurchasesAsWord(
                supplierId,
                wordStream,
                template,
                cancellationToken);

        if (!wordGeneration.IsSuccess)
        {
            return Result<bool>.Fail(wordGeneration.Message ?? "Something went wrong");
        }

        cancellationToken.ThrowIfCancellationRequested();
        
        wordStream.Position = 0;
        
        MiniPdf.ConvertToPdf(wordStream, destination);

        await destination.FlushAsync(cancellationToken);

        return Result<bool>.Ok(true);
    }

    private sealed record ItemTransactionSummary(
        string Name,
        string Description,
        string Uom,
        decimal TotalQuantity,
        decimal TotalPrice);
}