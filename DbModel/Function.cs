using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("Function")]
    public partial class Function
    {
           public Function(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public int InkPk {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public string FunctionName {get;set;}

           /// <summary>
           /// Desc:0启用 1不启用
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public int FunctionIsUse {get;set;}

    }
}
