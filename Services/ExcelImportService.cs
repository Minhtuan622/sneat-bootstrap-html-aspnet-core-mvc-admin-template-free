using AspnetCoreMvcFull.Models;
using AspnetCoreMvcFull.Repositories;
using OfficeOpenXml;

namespace AspnetCoreMvcFull.Services
{
  public class ExcelImportService(OrdersRepository repo)
  {
    public async Task<ImportResult>
      ImportOrders(IFormFile file)
    {
      var result = new ImportResult();

      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      using var stream = new MemoryStream();

      await file.CopyToAsync(stream);

      using var package = new ExcelPackage(stream);

      var sheet = package.Workbook.Worksheets[0];

      if (sheet.Dimension == null)
      {
        result.Errors.Add("File excel rỗng");

        return result;
      }

      var headers = new Dictionary<string, int>();

      for (var col = 1; col <= sheet.Dimension.Columns; col++)
      {
        var header = sheet.Cells[1, col].Text.Trim();

        if (!string.IsNullOrWhiteSpace(header))
        {
          headers[header] = col;
        }
      }

      var requiredHeaders = new[]
        {
          "Page_Id",
          "Mã_bài_viết",
          "Ad_Id",
          "Doanh_thu_đơn_hàng"
        };

      foreach (var header in requiredHeaders)
      {
        if (!headers.ContainsKey(header))
        {
          result.Errors.Add($"Thiếu cột: {header}");
        }
      }

      if (result.Errors.Count != 0)
      {
        return result;
      }

      var rowCount = sheet.Dimension.Rows;

      for (var row = 2; row <= rowCount; row++)
      {
        try
        {
          var pageId = sheet.Cells[row, headers["Page_Id"]].Text.Trim();
          var postId = sheet.Cells[row, headers["Mã_bài_viết"]].Text.Trim();
          var adId = sheet.Cells[row, headers["Ad_Id"]].Text.Trim();
          var revenueText = sheet.Cells[row, headers["Doanh_thu_đơn_hàng"]].Text.Trim();
          var createdAtText = sheet.Cells[row, headers["Thời_điểm_tạo_đơn"]].Text.Trim();

          if (string.IsNullOrWhiteSpace(postId))
          {
            throw new Exception("Post Id rỗng");
          }

          if (!decimal.TryParse(revenueText, out var revenue))
          {
            throw new Exception("Revenue không hợp lệ");
          }

          if (!DateTime.TryParse(createdAtText, out var createdAt))
          {
            throw new Exception("Ngày tạo không hợp lệ");
          }

          var exists =
            await repo.Exists(
              postId,
              adId,
              createdAt
            );

          if (exists)
          {
            result.Errors.Add($"Row {row}: Duplicate");
            result.FailedCount++;

            continue;
          }

          await repo.Insert(
            pageId,
            postId,
            adId,
            revenue,
            createdAt
          );

          result.SuccessCount++;
        }
        catch (Exception ex)
        {
          result.FailedCount++;

          result.Errors.Add(
            $"Row {row}: {ex.Message}"
          );
        }
      }

      return result;
    }
  }
}
