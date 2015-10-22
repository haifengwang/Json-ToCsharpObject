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

       private ClassStructureModel model;

       private JsonObject jobj;

       public CodeOrganization(string ns,string cName,string json)
       {
           this.NameSpaceName = ns;

           model = new ClassStructureModel();
           model.NamespeceName = ns;
           model.ClassName = cName;
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

                   if (jobj[jb].JsonType == JsonType.String)
                   {
                       model.PropertyCollection.Add(new PropertyModel("string", jb.ToString()));
                   }
                   if (jobj[jb].JsonType == JsonType.Boolean)
                   {
                       model.PropertyCollection.Add(new PropertyModel("bool", jb.ToString()));
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

                   }
                   if (jobj[jb].JsonType == JsonType.Array)
                   {
                       model.PropertyCollection.Add(new PropertyModel(string.Format("List<{0}>", jb.ToString() + "Model"), jb.ToString()));

                      

                       if (jobj[jb].Count > 0)
                       {
                           var submodel = new ClassStructureModel();
                           submodel.NamespeceName = this.NameSpaceName;
                           submodel.ClassName = jb.ToString() + "Model";
                           submodel.PropertyCollection = new List<PropertyModel>();
                           JsonObject jsobj = jobj[jb][0] as JsonObject;
                           InnerGenerateCode(jsobj, submodel);
                       }
                       else
                       {
                           var submodel = new ClassStructureModel();
                           submodel.NamespeceName = this.NameSpaceName;
                           submodel.ClassName = jb.ToString() + "Model";
                           InnerGenerateCode(null, submodel);
                       }

                   }
               }
           }

           new CreateCs().Produce(model);
       }
    }
}
