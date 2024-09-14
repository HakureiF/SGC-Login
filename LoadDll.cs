using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Seer
{
    internal class LoadDll
    {
        //已加载的DLL
        private static readonly Dictionary<string, Assembly> LoadedDlls = new Dictionary<string, Assembly>();
        //已处理的程序集
        private static readonly Dictionary<string, object> Assemblies = new Dictionary<string, object>();

        /// <summary> 在对程序集解释失败时触发 </summary>
        private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //获取加载失败的程序集的名称
            string assName = new AssemblyName(args.Name).FullName;
            //判断已加载的Dll集合中是否有已加载的同名程序集
            if (LoadedDlls.TryGetValue(assName, out Assembly ass) && ass != null)
            {
                LoadedDlls[assName] = null;
                return ass;
            }
            else
            {
                //抛出加载失败的异常
                throw new DllNotFoundException(assName);
            }
        }

        /// <summary> 注册资源中的dll </summary>
        public static void RegistDLL()
        {
            //获取当前项目的程序集
            Assembly ass = new StackTrace(0).GetFrame(1).GetMethod().Module.Assembly;
            //如果已处理程序集列表中包含此程序集则返回，否则将此程序集加入到已处理程序集列表中（Assemblies）
            if (Assemblies.ContainsKey(ass.FullName))
                return;
            else
                Assemblies.Add(ass.FullName, null);
            //绑定程序集加载失败事件
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
            //获取所有资源文件文件名
            string[] resources = ass.GetManifestResourceNames();
            //DLL文件名的正则表达式*.dll，如果是其它扩展名，可以修改该正则表达式
            var regex = new Regex("^.*\\.dll$", RegexOptions.IgnoreCase);
            foreach (string res in resources)
            {
                //如果是dll则加载
                if (regex.IsMatch(res))
                {
                    Stream s = ass.GetManifestResourceStream(res);
                    byte[] bytes = new byte[s.Length];
                    s.Read(bytes, 0, (int)s.Length);
                    Assembly loadedAss = Assembly.Load(bytes);
                    //判断是否已经加载
                    if (LoadedDlls.ContainsKey(loadedAss.FullName))
                        continue;
                    else
                        LoadedDlls[loadedAss.FullName] = loadedAss;
                }
            }
        }
    }
}
