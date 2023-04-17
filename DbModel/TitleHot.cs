using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("TitleHot")]
    public partial class TitleHot
    {
           public TitleHot(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public int IntPk {get;set;}
           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public string Title {get;set;}
           public int IsOk { get; set; }
           public int IsVideoOk { get; set; }
           public string Anser { get; set; }
    }
}
