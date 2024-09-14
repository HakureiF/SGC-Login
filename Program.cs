using Seer.DTO;
using Seer.Utils;
using System.Text.Json;

namespace Seer;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        LoadDll.RegistDLL();
        CreateFileIfNotExists("store.json");
        var store = StoreUtil.getStore();
        if (store != null)
        {
            if (store.url_battle != null)
            {
                Constant.Host = store.url_battle;
            }
            if (store.url_match != null)
            {
                Constant.MatchHost = store.url_match;
            }
        }

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        Application.EnableVisualStyles();
        ApplicationConfiguration.Initialize();
        Application.Run(new Main());
    }

    private static void CreateFileIfNotExists(string fileName)
    {
        if (!File.Exists(Application.StartupPath + fileName))
        {
            FileStream fs1 = new FileStream(Application.StartupPath + fileName, FileMode.Create, FileAccess.Write);//创建写入文件
            StreamWriter sw = new StreamWriter(fs1);

            var store = new Store();
            sw.WriteLine(JsonSerializer.Serialize(store));

            sw.Close();
            fs1.Close();
        }
    }
}