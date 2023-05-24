using System.Text.Json.Serialization;

namespace AppMenuSharper
{
    public partial class Form1
    {
        class LauncherItem
        {

            public string exe_path { get; set; }
            public string title { get; set; }
            public string env_ini { get; set; }
            public string tmpicon { get; set; }
            
            

            public  LauncherItem(string exe_path)
            {
                int length = exe_path.LastIndexOf('.') - exe_path.LastIndexOf('\\');
                string title = exe_path.Substring(exe_path.LastIndexOf('\\')+1, length-1);
                //new LauncherItem(exe_path,title);
                this.title = title;
                this.exe_path = exe_path;
                this.env_ini = "";
                this.tmpicon = "";
            }

            [JsonConstructorAttribute]
            public LauncherItem(string exe_path,string title)
            {
                this.exe_path = exe_path;
                this.title = title;
                this.env_ini = "";
                this.tmpicon = "";
            }
        }
    }
}
