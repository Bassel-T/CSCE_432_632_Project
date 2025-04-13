using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemindMe.Services
{
    public static class SystemConstants
    {
        // Replace [IPv4] with the actual IPv4 address of your machine on the network
        public static readonly string BACKEND_URL = "http://192.168.1.98:5128/api";

        // Where the user's session is stored
        public static readonly string USER_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "user");
    }
}
