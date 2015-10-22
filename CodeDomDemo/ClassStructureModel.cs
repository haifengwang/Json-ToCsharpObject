using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeDomDemo
{
    /// <summary>
    /// 类结构模型
    /// </summary>
    internal class ClassStructureModel
    {
       /// <summary>
       /// 命名空间名
       /// </summary>
       public string NamespeceName { get; set; }

       /// <summary>
       /// 类名
       /// </summary>
       public string ClassName { get; set; }

       /// <summary>
       /// 属性集合
       /// </summary>
       public List<PropertyModel> PropertyCollection { get; set; }
    }

   public class PropertyModel
   {

       public PropertyModel(string typeStr,string propertyName)
       {
           this.PropertyText = string.Format("\t\tpublic {0} {1} {{ get; set; }}\n",typeStr,propertyName);
       }

       public string PropertyText { get; private set; }
   }
}
