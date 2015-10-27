using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.Json;

namespace CodeDomDemo
{
    class Program
    {
      
        static void Main(string[] args)
        {

          
            string content;
            using(System.IO.StreamReader reader = new System.IO.StreamReader(@"F:\datalist.js",Encoding.UTF8))
            {
                content = reader.ReadToEnd();
            }

            CodeOrganization code = new CodeOrganization("com.onskylines", "applyResultModel", content, @"F:\Baojia");
            code.GenerateCode();
           
        }
    }
}
