using Microsoft.VisualBasic.Devices;
using Seer.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Seer.Utils
{
    public static class PetUtil
    {
        public static Dictionary<string, Dictionary<string, string>>? specialElfHeads { get; set; }
        public static Dictionary<int, string>? petsData { get; set; }

        public static void InitPetsData()
        {
            string fileName = "pets_jsondata.json";
            if (File.Exists(Application.StartupPath + fileName))
            {
                FileStream fsr = new FileStream(Application.StartupPath + fileName, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fsr);
                petsData = JsonSerializer.Deserialize<Dictionary<int, string>>(sr.ReadToEnd());

                fsr.Close();
                sr.Close();
            }

            string fileName2 = "special_petheads.json";
            if (File.Exists(Application.StartupPath + fileName2))
            {
                FileStream fsr = new FileStream(Application.StartupPath + fileName2, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fsr);
                specialElfHeads = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(sr.ReadToEnd());

                fsr.Close();
                sr.Close();
            }
        }


        public const string HeadTaomee = "https://seerh5.61.com/resource/assets/pet/head/";
        public const string HeadSGC = "https://cdn.imrightchen.live/img/elf-head/";
        public static string handleHeadUrl(int id)
        {
            string id_str = id.ToString();
            if (specialElfHeads != null && specialElfHeads.ContainsKey(id_str))
            {
                return HeadTaomee + specialElfHeads[id_str]["replace"] + ".png";
            }
            else if (id > 5000)
            {
                return HeadSGC + id_str + ".png";
            }
            return HeadTaomee + id_str + ".png";
        }

        public static bool CheckBag(List<Pet>? pets)
        {
            if (pets == null)
            {
                MessageBox.Show(@"获取背包数据失败！");
                return false;
            }
            if (pets.Count < 9)
            {
                MessageBox.Show(@"请至少在背包中携带9只精灵！");
                return false;
            }
            for (int i = 0; i < pets.Count; i++)
            {
                if (pets[i].id < 6)
                {
                    MessageBox.Show(@"请勿携带序号小于6的精灵！");
                    return false;
                }
                if (pets[i].level < 100)
                {
                    MessageBox.Show(@"请勿携带不满级的精灵！");
                    return false;
                }
                for (int j = i + 1; j < pets.Count; j++)
                {
                    if (pets[i].id == pets[j].id && pets[i].effectID == pets[j].effectID)
                    {
                        MessageBox.Show(@"请勿携带相同精灵！");
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
