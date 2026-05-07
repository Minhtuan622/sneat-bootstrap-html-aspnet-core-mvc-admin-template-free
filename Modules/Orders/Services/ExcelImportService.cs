using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repositories;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;

namespace AspnetCoreMvcFull.Services
{
  public class ExcelImportService(OrdersRepository repo, ErrorLogService errorLogService)
  {
    public async Task<ImportResult> ImportOrders(IFormFile file)
    {
      using var stream = new MemoryStream();
      await file.CopyToAsync(stream);
      return await ImportOrders(stream);
    }

    public async Task<ImportResult> ImportOrdersFromPath(string filePath)
    {
      await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
      using var ms = new MemoryStream();
      await fs.CopyToAsync(ms);
      return await ImportOrders(ms);
    }

    private async Task<ImportResult> ImportOrders(Stream input)
    {
      var result = new ImportResult();
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      input.Position = 0;
      using var package = new ExcelPackage(input);
      var sheet = package.Workbook.Worksheets[0];
      if (sheet.Dimension == null) { result.Errors.Add("File excel rỗng"); return result; }

      var headers = new Dictionary<string, int>();
      for (var col = 1; col <= sheet.Dimension.Columns; col++) { var header = sheet.Cells[1, col].Text.Trim(); if (!string.IsNullOrWhiteSpace(header)) headers[header] = col; }
      var requiredHeaders = new[] { "Page_Id", "Mã_bài_viết", "Ad_Id", "Doanh_thu_đơn_hàng", "Thời_điểm_tạo_đơn" };
      foreach (var header in requiredHeaders) if (!headers.ContainsKey(header)) result.Errors.Add($"Thiếu cột: {header}");
      if (result.Errors.Count != 0) return result;

      for (var row = 2; row <= sheet.Dimension.Rows; row++)
      {
        try
        {
          var pageId = sheet.Cells[row, headers["Page_Id"]].Text.Trim();
          var postId = sheet.Cells[row, headers["Mã_bài_viết"]].Text.Trim();
          var adId = sheet.Cells[row, headers["Ad_Id"]].Text.Trim();
          var revenueText = sheet.Cells[row, headers["Doanh_thu_đơn_hàng"]].Text.Trim();
          var createdAtText = sheet.Cells[row, headers["Thời_điểm_tạo_đơn"]].Text.Trim();
          if (string.IsNullOrWhiteSpace(postId)) throw new Exception("Post Id rỗng");
          if (!decimal.TryParse(revenueText, out var revenue)) throw new Exception("Revenue không hợp lệ");
          if (!DateTime.TryParse(createdAtText, out var createdAt)) throw new Exception("Ngày tạo không hợp lệ");

          await repo.Insert(pageId, postId, adId, revenue, createdAt);
          result.SuccessCount++;
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
          result.FailedCount++;
          result.Errors.Add($"Row {row}: Duplicate");
        }
        catch (Exception ex)
        {
          result.FailedCount++;
          result.Errors.Add($"Row {row}: {ex.Message}");
          await errorLogService.Log(ex, "ExcelImportService.ImportOrders");
        }
      }
      return result;
    }
  }
}
