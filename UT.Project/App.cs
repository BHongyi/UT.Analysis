using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UT.Project
{
    public class App
    {
        public void Execute(int i,string j,string k)
        {
            if (i == 1 && j.StartsWith("1") && k.Length >= 2 && k.Substring(1, 1) == "3")
            {
            }

            if (i != 1)
            {
            }

            if (i == 1 && !j.StartsWith("1"))
            {
            }

            if (i == 1 && j.StartsWith("1") && k.Length < 2)
            {
            }

            if (i == 1 && j.StartsWith("1") && k.Length >= 2 && (k.Substring(1, 1) == "3"))
            {
            }

            if (i == 1 && j.StartsWith("1") && k.Length >= 2 && (k.Substring(1, 1) != "3" && k.Substring(1, 1) == "4"))
            {
            }

            if (i == 1 && j.StartsWith("1") && k.Length >= 2 && (k.Substring(1,1) == "3" || k.Substring(1, 1) == "4"))
            {
                int s = 1;
            }

        }
    }
}
