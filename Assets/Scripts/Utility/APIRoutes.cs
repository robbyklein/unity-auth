public class APIRoutes {
  private static readonly string DEFAULT_ROOT = "http://localhost:3000";

  public static string ROOT {
    get {
      return System.Environment.GetEnvironmentVariable("API_ROOT") ?? DEFAULT_ROOT;
    }
  }

  public static string LOGIN => $"{ROOT}/login";
  public static string SIGNUP => $"{ROOT}/signup";
  public static string VALIDATE => $"{ROOT}/validate";
}