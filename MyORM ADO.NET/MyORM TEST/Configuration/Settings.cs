using System.Text.Json;

namespace MyORM_TEST.Configuration;

public class Settings
{
   private static AppSettings? AppSettings { get; set; }
   private static string _appSettingsPath = "appsettings.json";

   private Settings()
   {
      if (!File.Exists(_appSettingsPath)) throw new FileNotFoundException("app settings file not found");

      using (var appsettingJson = File.OpenRead(_appSettingsPath))
         AppSettings = JsonSerializer.Deserialize<AppSettings>(appsettingJson);
      
   }

   public static AppSettings GetAppSettings()
   {
      if (AppSettings is null)
         new Settings();
      
      return AppSettings!;
   } 

}