using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.CodeDom;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.IO;

namespace CodeDomDemo
{
    /// <summary>
    /// 生成代码类
    /// <remarks>一定不能用单例，否则会发生代码结构重复的情况</remarks>
    /// </summary>
  internal class CreateCs
    {
      
      private CodeCompileUnit unit = new CodeCompileUnit();
     
      public void Produce(ClassStructureModel model)
      {
          
          CodeNamespace theNamespace = new CodeNamespace(model.NamespeceName);
          unit.Namespaces.Add(theNamespace);

          CodeNamespaceImport SystemImport = new CodeNamespaceImport("System");
          theNamespace.Imports.Add(SystemImport);

          CodeNamespaceImport collectionsImport = new CodeNamespaceImport("System.Collections.Generic");
          theNamespace.Imports.Add(collectionsImport);

          CodeTypeDeclaration mClass = new CodeTypeDeclaration(model.ClassName);
          mClass.IsClass = true;
          mClass.TypeAttributes = TypeAttributes.Public;
          theNamespace.Types.Add(mClass);

          //通过snippet 生成属性
          if (model.PropertyCollection != null)
          {
              foreach (var item in model.PropertyCollection)
              {
                  CodeSnippetTypeMember snippet = new CodeSnippetTypeMember();

                  //snippet.Comments.Add(new CodeCommentStatement("this is integer property", false));
                  snippet.Text = item.PropertyText;
                  mClass.Members.Add(snippet);
              }
          }
          

          /*
           *  .net 2.0 时代属性生成方式
          CodeMemberField myField = new CodeMemberField("System.Int32", "_age");
          mClass.Members.Add(myField);

          //属性
          CodeMemberProperty ageAtrr = new CodeMemberProperty();

          ageAtrr.Name = "Age";

          ageAtrr.Type = new CodeTypeReference("System.Int32");

          ageAtrr.Attributes = MemberAttributes.Public | MemberAttributes.Final;

          ageAtrr.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_age")));

          ageAtrr.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_age"),

               new CodePropertySetValueReferenceExpression()));
          mClass.Members.Add(ageAtrr);
           * */
          if (!System.IO.Directory.Exists(model.FileDirPath))
          {
              Directory.CreateDirectory(model.FileDirPath);
          }

          if (!model.FileDirPath.EndsWith("/"))
          {
              model.FileDirPath = model.FileDirPath + "/";
          }

          IndentedTextWriter tw = new IndentedTextWriter(new StreamWriter(string.Format("{0}{1}.cs",model.FileDirPath,model.ClassName), false), "   ");
          CodeDomProvider provide = new CSharpCodeProvider();

          provide.GenerateCodeFromCompileUnit(unit, tw, new CodeGeneratorOptions());
          tw.Close();
      }

    }
}
