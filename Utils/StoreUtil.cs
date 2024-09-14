using Seer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Seer.Utils
{
    public class StoreUtil
    {

        public static Store? getStore()
        {
            string fileName = "store.json";
            if (!File.Exists(Application.StartupPath + fileName))
            {
                MessageBox.Show("配置文件store.json丢失，请重启登录器");
                return null;
            }
            else
            {
                FileStream fsr = new FileStream(Application.StartupPath + fileName, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fsr);
                var store = JsonSerializer.Deserialize<Store>(sr.ReadToEnd());
                fsr.Close();
                sr.Close();
                return store;
            }
        }

        public static void setStore(Store store)
        {
            string fileName = "store.json";

            FileStream fsw = new FileStream(Application.StartupPath + fileName, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fsw);
            string text = JsonSerializer.Serialize(store);
            sw.Write(text);

            sw.Close();
            fsw.Close();
        }
    }
}
