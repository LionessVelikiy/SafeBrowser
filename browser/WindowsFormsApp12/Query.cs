using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp12
{
    class Query
    {
        public string id;

        public string value = "";
        public string type;
        public bool isBlock = false;
        public Query(string data)
        {
            string[] param = data.Split('~');
            if (param.Length > 2)
            {
                if (param[0] != "")
                {
                    this.value = param[0];
                    this.type = param[1];
                    this.id = param[2];
                    this.isBlock = param[3] == "BLOCK";
                }


            }
        }
    }
}
