using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.Json;

namespace CodeDomDemo
{
    /// <summary>
    /// 代码的组织结构
    /// </summary>
   public class CodeOrganization
    {

       public string JsonContent { get; private set; }

       public string NameSpaceName { get; private set; }

       public string DirPath { get; set; }

       private ClassStructureModel model;

       private JsonObject jobj;

       public CodeOrganization(string ns,string cName,string json,string dirPath)
       {
           this.NameSpaceName = ns;
           this.DirPath = dirPath;

           model = new ClassStructureModel();
           model.NamespeceName = ns;
           model.ClassName = cName;
           model.FileDirPath = dirPath;
           
           model.PropertyCollection = new List<PropertyModel>();
           jobj = JsonValue.Parse(json) as JsonObject;
          
       }

       public void GenerateCode()
       {
           InnerGenerateCode(jobj,model);
       }

       private void InnerGenerateCode(JsonObject jobj,ClassStructureModel model)
       {
           if (jobj != null)
           {
               foreach (var jb in jobj.Keys)
               {
                   if (jobj[jb] == null)
                   {
                       continue;
                   }

                   if (jobj[jb].JsonType == JsonType.String)
                   {
                       model.PropertyCollection.Add(new PropertyModel("string", jb.ToString()));
                       continue;
                   }
                   if (jobj[jb].JsonType == JsonType.Boolean)
                   {
                       model.PropertyCollection.Add(new PropertyModel("bool", jb.ToString()));
                       continue;
                   }
                   if (jobj[jb].JsonType == JsonType.Number)
                   {
                       //需要判断 int型还是 double,这种判断是不准确的
                       if (Regex.IsMatch(jobj[jb].ToString(), @"^\d+\.\d+$"))
                       {
                           model.PropertyCollection.Add(new PropertyModel("double", jb.ToString()));
                       }
                       else
                       {
                           model.PropertyCollection.Add(new PropertyModel("int", jb.ToString()));
                       }
                       continue;

                   }
                   //数组
                   if (jobj[jb].JsonType == JsonType.Array)
                   {
                       model.PropertyCollection.Add(new PropertyModel(string.Format("List<{0}>", jb.ToString() + "Model"), jb.ToString()));

                      

                       if (jobj[jb].Count > 0)
                       {
                           JsonObject jsobj = jobj[jb][0] as JsonObject;
                           subGenerateStep(jsobj, jb);
                       }
                       else
                       {
                           var submodel = new ClassStructureModel();
                           submodel.NamespeceName = this.NameSpaceName;
                           submodel.ClassName = jb.ToString() + "Model";
                           submodel.FileDirPath = this.DirPath;
                           InnerGenerateCode(null, submodel);
                       }
                       continue;

                   }
                   //对象
                   if (jobj[jb].JsonType == JsonType.Object)
                   {

                       JsonObject jsobj = jobj[jb] as JsonObject;
                       subGenerateStep(jsobj, jb);
                       continue;
                   }
               }
           }

           new CreateCs().Produce(model);
       }


       private void subGenerateStep(JsonObject jsobj, string jb)
       {
           var submodel = new ClassStructureModel();
           submodel.NamespeceName = this.NameSpaceName;
           submodel.ClassName = jb.ToString() + "Model";
           submodel.FileDirPath = this.DirPath;
           submodel.PropertyCollection = new List<PropertyModel>();
          
           InnerGenerateCode(jsobj, submodel);
       }
    }
}
