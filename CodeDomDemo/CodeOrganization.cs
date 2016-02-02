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

                   //对象

                   #region 处理对象
                   //对象
                   if (jobj[jb].JsonType == JsonType.Object)
                   {

                       JsonObject jsobj = jobj[jb] as JsonObject;
                       
                       //如何针对空对象处理呢？ object ?? 新建一个空对象类[目前是新建空对象类]
                       if (jsobj != null && jsobj.Count > 0)
                       {
                           model.PropertyCollection.Add(new PropertyModel(string.Format("{0}", jb.ToString() + "Model"), jb.ToString()));

                           subGenerateStep(jsobj, jb);
                       }
                       else
                       {
                           model.PropertyCollection.Add(new PropertyModel(string.Format("{0}", "object"), jb.ToString()));
                       }

                       continue;
                   }
                   #endregion 

                   //数组 

                   #region 处理数组
                   if (jobj[jb].JsonType == JsonType.Array)
                   {
                      
                       if (jobj[jb].Count > 0)
                       {
                           JsonObject jsobj = jobj[jb][0] as JsonObject;
                           //jsobj==null,就是简单对象，简单对象转换为数组
                           if (jsobj != null)
                           {
                               model.PropertyCollection.Add(new PropertyModel(string.Format("List<{0}>", jb.ToString() + "Model"), jb.ToString()));
                               subGenerateStep(jsobj, jb);
                           }
                           else 
                           {
                               //将简单类型的数组转换直接转换为数组
                               InnerSimpleObjInArray(jobj, model, jb);
                           }
                          
                       }
                       else
                       {
                           //将空数组转换为 object[]数组  
                           model.PropertyCollection.Add(new PropertyModel("object[]", jb.ToString()));
                           continue;                         
                        
                       }
                       continue;

                   }
                   #endregion 

                   //处理简单类型
                   InnerSimpleObj(jobj, model, jb);
               }
           }

           new CreateCs().Produce(model);
       }

       private void InnerSimpleObj(JsonObject jobj, ClassStructureModel model, string jb)
       {
           if (jobj[jb].JsonType == JsonType.String)
           {
               model.PropertyCollection.Add(new PropertyModel("string", jb.ToString()));
               // continue;
           }
           if (jobj[jb].JsonType == JsonType.Boolean)
           {
               model.PropertyCollection.Add(new PropertyModel("bool", jb.ToString()));
               // continue;
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
               // continue;

           }
       }

       private void InnerSimpleObjInArray(JsonObject jobj, ClassStructureModel model, string jb)
       {
           if (jobj[jb][0].JsonType == JsonType.String)
           {
               model.PropertyCollection.Add(new PropertyModel("String[]", jb.ToString()));
               //continue;
           }

           if (jobj[jb][0].JsonType == JsonType.Boolean)
           {
               model.PropertyCollection.Add(new PropertyModel("bool[]", jb.ToString()));
               //continue;
           }
           if (jobj[jb][0].JsonType == JsonType.Number)
           {
               //需要判断 int型还是 double,这种判断是不准确的
               if (Regex.IsMatch(jobj[jb][0].ToString(), @"^\d+\.\d+$"))
               {
                   model.PropertyCollection.Add(new PropertyModel("double[]", jb.ToString()));
               }
               else
               {
                   model.PropertyCollection.Add(new PropertyModel("int[]", jb.ToString()));
               }
               // continue;

           }
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
