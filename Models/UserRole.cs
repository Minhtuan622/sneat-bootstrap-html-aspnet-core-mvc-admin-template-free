namespace AspnetCoreMvcFull.Models;

public static class UserRole
{
  public const string Admin = "Admin";
  public const string Manager = "Manager";
  public const string Operator = "Operator";
  public const string Viewer = "Viewer";

  public static readonly string[] All = [Admin, Manager, Operator, Viewer];
}
