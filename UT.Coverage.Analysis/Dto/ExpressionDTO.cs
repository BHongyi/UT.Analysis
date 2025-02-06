using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UT.Coverage.Analysis.Dto
{
    public class ExpressionDTO
    {
        public string ID { get; set; }
        /// <summary>
        /// OR,AND
        /// </summary>
        public string JudgeFlg { get; set; }

        /// <summary>
        /// ConditionStr include GUID
        /// </summary>
        public string CondtionStr { get; set; }

        /// <summary>
        /// ConditionList
        /// </summary>
        public List<string> ConditionList { get; set; }

        public List<ExpressionDTO> ChildExpressionDTO  = new List<ExpressionDTO>();
    }
}
