using AspnetCoreMvcFull.Repositories;

namespace AspnetCoreMvcFull.Services;

public class ErrorLogService(ErrorLogRepository repo, ILogger<ErrorLogService> logger)
{
  public async Task Log(Exception ex, string source)
  {
    logger.LogError(ex, "{Source}: {Message}", source, ex.Message);
    try { await repo.Create(ex.Message, ex.StackTrace, source); }
    catch { /* avoid crashing caller */ }
  }
}
