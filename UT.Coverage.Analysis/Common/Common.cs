using System.Linq.Expressions;
using UT.Coverage.Analysis.Dto;

namespace UT.Coverage.Analysis.Common
{
    public static class Common
    {
        public static string HandleExpression(string expression, ExpressionDTO exp)
        {
            exp.ID = Constant.CHILDFLG + Guid.NewGuid().ToString();
            exp.CondtionStr = expression;

            //Replace parentheses
            string tempParenthese = exp.CondtionStr;
            ReplaceParentheses(ref tempParenthese);
            exp.CondtionStr = tempParenthese;

            while (exp.CondtionStr.IndexOf('(') > -1)
            {
                int startIndex = exp.CondtionStr.IndexOf("(");
                int endIndex = GetEndIndex(exp.CondtionStr);
                string tempStartExpression = exp.CondtionStr.Substring(0, exp.CondtionStr.IndexOf("("));
                string tempEndExpression = exp.CondtionStr.Substring(endIndex + 1);
                exp.ChildExpressionDTO.Add(new());
                exp.CondtionStr = tempStartExpression + HandleExpression(exp.CondtionStr.Substring(startIndex + 1, endIndex - startIndex - 1), exp.ChildExpressionDTO.Last()) + tempEndExpression;
            }

            if (exp.CondtionStr.IndexOf("|") > -1 && exp.CondtionStr.IndexOf("&") > -1 && exp.CondtionStr.IndexOf("|") < exp.CondtionStr.IndexOf("&"))
            {
                exp.JudgeFlg = Constant.OR;
            }
            else if (exp.CondtionStr.IndexOf("|") > -1 && exp.CondtionStr.IndexOf("&") > -1 && exp.CondtionStr.IndexOf("|") > exp.CondtionStr.IndexOf("&"))
            {
                exp.JudgeFlg = Constant.AND;
            }
            else if (exp.CondtionStr.IndexOf("|") > -1 && exp.CondtionStr.IndexOf("&") == -1)
            {
                exp.JudgeFlg = Constant.OR;
            }
            else if (exp.CondtionStr.IndexOf("|") == -1 && exp.CondtionStr.IndexOf("&") > -1)
            {
                exp.JudgeFlg = Constant.AND;
            }

            exp.CondtionStr = RestoreParentheses(exp.CondtionStr);

            exp.ConditionList = exp.CondtionStr.Split(exp.JudgeFlg).ToList();

            return exp.ID;
        }

        public static void HandleFalseFlgForChild(ExpressionDTO exp) 
        {
            for (int i = 0; i < exp.ConditionList.Count; i++)
            {
                if (exp.ConditionList[i].Trim().StartsWith(Constant.NOT + Constant.CHILDFLG))
                {
                    exp.CondtionStr = exp.CondtionStr.Replace(exp.ConditionList[i], exp.ConditionList[i].Trim().TrimStart(Constant.NOT.ToCharArray()));
                    exp.ConditionList[i] = exp.ConditionList[i].Trim().TrimStart(Constant.NOT.ToCharArray());
                    ExpressionDTO childExp = exp.ChildExpressionDTO.First(o => o.ID == exp.ConditionList[i]);

                    if (childExp.JudgeFlg == Constant.OR)
                    {
                        childExp.JudgeFlg = Constant.AND;

                        childExp.CondtionStr = childExp.CondtionStr.Replace(Constant.OR,Constant.AND);
                    }
                    else
                    {
                        childExp.JudgeFlg = Constant.OR;

                        childExp.CondtionStr = childExp.CondtionStr.Replace(Constant.AND, Constant.OR);
                    }

                    for (int j = 0; j < childExp.ConditionList.Count; j++)
                    {
                        string tempFalseCondition = string.Empty;
                        if (childExp.ConditionList[j].IndexOf(Constant.CHILDFLG) > -1)
                        {
                            if (childExp.ConditionList[j].Trim().StartsWith(Constant.NOT))
                            {
                                tempFalseCondition = childExp.ConditionList[j].Trim().TrimStart(Constant.NOT.ToCharArray());
                            }
                            else 
                            {
                                tempFalseCondition = Constant.NOT + childExp.ConditionList[j];
                            }
                        }
                        else
                        {
                            tempFalseCondition = GetNotExpression(childExp.ConditionList[j]);
                        }
                        childExp.CondtionStr = childExp.CondtionStr.Replace(childExp.ConditionList[j], tempFalseCondition);
                        childExp.ConditionList[j] = tempFalseCondition;
                    }

                    if (childExp.CondtionStr.IndexOf(Constant.CHILDFLG) > -1)
                    {
                        HandleFalseFlgForChild(childExp);
                    }
                }
            }
        }

        private static void ReplaceParentheses(ref string condition)
        {
            if (condition.IndexOf(".IsNullOrEmpty(") > -1)
            {
                //Replace )
                int endIndex = GetEndIndex(condition, condition.IndexOf(".IsNullOrEmpty("));
                string tempFrontCondition = condition.Substring(0,endIndex);
                string tempAfterCondition = condition.Substring(endIndex + 1);
                condition = tempFrontCondition + Constant.REPLACEFLGEND + tempAfterCondition;

                //Replace (
                tempFrontCondition = condition.Substring(0, condition.IndexOf(".IsNullOrEmpty(") + 14);
                tempAfterCondition = condition.Substring(condition.IndexOf(".IsNullOrEmpty(") + 15);
                condition = tempFrontCondition + Constant.REPLACEFLGSTART + tempAfterCondition;
            }
            if (condition.IndexOf(".StartsWith(") > -1)
            {
                //Replace )
                int endIndex = GetEndIndex(condition, condition.IndexOf(".StartsWith("));
                string tempFrontCondition = condition.Substring(0, endIndex);
                string tempAfterCondition = condition.Substring(endIndex + 1);
                condition = tempFrontCondition + Constant.REPLACEFLGEND + tempAfterCondition;
                //Replace (
                tempFrontCondition = condition.Substring(0, condition.IndexOf(".StartsWith(") + 11);
                tempAfterCondition = condition.Substring(condition.IndexOf(".StartsWith(") + 12);
                condition = tempFrontCondition + Constant.REPLACEFLGSTART + tempAfterCondition;
            }
            if(condition.IndexOf(".Substring(") > -1)
            {
                //Replace )
                int endIndex = GetEndIndex(condition, condition.IndexOf(".Substring("));
                string tempFrontCondition = condition.Substring(0, endIndex);
                string tempAfterCondition = condition.Substring(endIndex + 1);
                condition = tempFrontCondition + Constant.REPLACEFLGEND + tempAfterCondition;
                //Replace (
                tempFrontCondition = condition.Substring(0, condition.IndexOf(".Substring(") + 10);
                tempAfterCondition = condition.Substring(condition.IndexOf(".Substring(") + 11);
                condition = tempFrontCondition + Constant.REPLACEFLGSTART + tempAfterCondition;
            }
            if (condition.IndexOf(".Any(") > -1)
            {
                //Replace )
                int endIndex = GetEndIndex(condition, condition.IndexOf(".Any("));
                string tempFrontCondition = condition.Substring(0, endIndex);
                string tempAfterCondition = condition.Substring(endIndex + 1);
                condition = tempFrontCondition + Constant.REPLACEFLGEND + tempAfterCondition;
                //Replace (
                tempFrontCondition = condition.Substring(0, condition.IndexOf(".Any(") + 4);
                tempAfterCondition = condition.Substring(condition.IndexOf(".Any(") + 5);
                condition = tempFrontCondition + Constant.REPLACEFLGSTART + tempAfterCondition;
            }
            foreach (string specialMethod in Constant.SPECTIALMETHOD)
            {
                if (condition.IndexOf(specialMethod) > -1)
                {
                    ReplaceParentheses(ref condition);
                }
            }
        }

        private static string RestoreParentheses(string condition)
        {
            condition = condition.Replace(Constant.REPLACEFLGSTART,"(").Replace(Constant.REPLACEFLGEND,")");

            return condition;
        }

        private static int GetEndIndex(string condition,int startPlace = 0)
        {
            Stack<char> stack = new Stack<char>();
            Dictionary<char, char> parenthesesMap = new Dictionary<char, char>()
            {
                { ')', '(' }
            };

            bool startFlg = false;

            for (int i = startPlace; i < condition.Length; i++)
            {
                if (condition[i] == '(')
                {
                    stack.Push(condition[i]);
                    startFlg = true;
                }
                else if (condition[i] == ')')
                {
                    if (stack.Count == 1 && startFlg)
                    {
                        return i;
                    }
                    stack.Pop();
                }
            }

            return -1; // 如果栈为空，则所有括号正确匹配，返回true；否则，返回false
        }

        private static string GetNotExpression(string condition) 
        {
            if (condition.IndexOf("==") > -1)
            {
                condition = condition.Replace("==", "!=");
            }
            else if (condition.IndexOf(">=") > -1)
            {
                condition = condition.Replace(">=", "<");
            }
            else if (condition.IndexOf("<=") > -1)
            {
                condition = condition.Replace("<=", ">");
            }
            else if (condition.IndexOf(">") > -1)
            {
                condition = condition.Replace(">", "<=");
            }
            else if (condition.IndexOf("<") > -1)
            {
                condition = condition.Replace("<", ">=");
            }
            else
            {
                foreach (string specialMethod in Constant.SPECTIALMETHOD)
                {
                    if (condition.IndexOf(specialMethod) > -1)
                    {
                        if (condition.IndexOf(Constant.NOT) > -1)
                        {
                            condition = condition.Replace(Constant.NOT, string.Empty);
                        }
                        else
                        {
                            condition = Constant.NOT + condition;
                        }
                        return condition;
                    }
                }
            }
            return condition;
        }

        private static string GetNotExpressionByChild(ExpressionDTO exp) 
        {
            if (exp.JudgeFlg == Constant.OR)
            {
                string tempCondition = string.Empty;
                for (int i = 0; i < exp.ConditionList.Count; i++)
                {
                    if (exp.ConditionList[i].IndexOf(Constant.CHILDFLG) > -1)
                    {
                        tempCondition += GetNotExpressionByChild(exp.ChildExpressionDTO.First(o => o.ID == exp.ConditionList[i].Trim()));
                    }
                    else
                    {
                        tempCondition += GetNotExpression(exp.ConditionList[i]);
                    }

                    if (i < exp.ConditionList.Count - 1)
                    {
                        tempCondition += Constant.AND;
                    }
                }

                return tempCondition;
            }
            else
            {
                if (exp.ConditionList[0].IndexOf(Constant.CHILDFLG) > -1)
                {
                    return GetNotExpressionByChild(exp.ChildExpressionDTO.First(o => o.ID == exp.ConditionList[0].Trim()));
                }
                else
                {
                    return GetNotExpression(exp.ConditionList[0]);
                }
            }
        }

        private static string GetExpressionByChild(ExpressionDTO exp)
        {
            if (exp.JudgeFlg == Constant.AND)
            {
                string tempCondition = string.Empty;
                for (int i = 0; i < exp.ConditionList.Count; i++)
                {
                    if (exp.ConditionList[i].IndexOf(Constant.CHILDFLG) > -1)
                    {
                        tempCondition += GetExpressionByChild(exp.ChildExpressionDTO.First(o => o.ID == exp.ConditionList[i].Trim()));
                    }
                    else
                    {
                        tempCondition += exp.ConditionList[i];
                    }

                    if (i < exp.ConditionList.Count - 1)
                    {
                        tempCondition += Constant.AND;
                    }
                }

                return tempCondition;
            }
            else
            {
                if (exp.ConditionList[0].IndexOf(Constant.CHILDFLG) > -1)
                {
                    return GetExpressionByChild(exp.ChildExpressionDTO.First(o => o.ID == exp.ConditionList[0].Trim()));
                }
                else
                {
                    return exp.ConditionList[0];
                }
            }
        }

        public static void GetPatterns(ExpressionDTO exp, List<string> results,string parentCondition = null) 
        {
            if (exp.JudgeFlg == Constant.OR)
            {
                string falseExpression = GetNotExpressionByChild(exp);
                if (!string.IsNullOrEmpty(parentCondition) && parentCondition.IndexOf(exp.ID) > -1)
                {
                    falseExpression = parentCondition.Replace(exp.ID, "(" + falseExpression + ")");
                }

                results.Add(falseExpression);

                for (int i = 0; i < exp.ConditionList.Count; i++)
                {
                    string tempCondition = string.Empty;

                    for (int j = 0; j < i; j++)
                    {
                        //set data of j < i to Not and j = i to True
                        if (j < i)
                        {
                            if (exp.ConditionList[j].IndexOf(Constant.CHILDFLG) > -1)
                            {
                                tempCondition += "(" + GetNotExpressionByChild(exp.ChildExpressionDTO.First(o => o.ID == exp.ConditionList[j].Trim())) + ")" + Constant.AND;
                            }
                            else
                            {
                                tempCondition += GetNotExpression(exp.ConditionList[j]) + Constant.AND;
                            }
                        }
                    }

                    tempCondition += exp.ConditionList[i];

                    //Replace child Guid
                    if (!string.IsNullOrEmpty(parentCondition) && parentCondition.IndexOf(exp.ID) > -1)
                    {
                        tempCondition = parentCondition.Replace(exp.ID, "(" + tempCondition + ")");
                    }

                    //if it's a expression that have child element, it will expand the child element
                    if (exp.ConditionList[i].IndexOf(Constant.CHILDFLG) > -1)
                    {
                        GetPatterns(exp.ChildExpressionDTO.First(o => o.ID == exp.ConditionList[i].Trim()), results, tempCondition);
                    }
                    else 
                    {
                        results.Add(tempCondition);
                    }
                }
            }
            else 
            {
                string trueExpression = GetExpressionByChild(exp);
                if (!string.IsNullOrEmpty(parentCondition) && parentCondition.IndexOf(exp.ID) > -1)
                {
                    trueExpression = parentCondition.Replace(exp.ID, "(" + trueExpression + ")");
                }

                results.Add(trueExpression);

                for (int i = 0; i < exp.ConditionList.Count; i++)
                {
                    string tempCondition = string.Empty;

                    for (int j = 0; j <= i; j++)
                    {
                        //set data of j < i to True and j = i to Not
                        if (j < i)
                        {
                            if (exp.ConditionList[j].IndexOf(Constant.CHILDFLG) > -1)
                            {
                                tempCondition += "(" + GetExpressionByChild(exp.ChildExpressionDTO.First(o => o.ID == exp.ConditionList[j].Trim())) + ")" + Constant.AND;
                            }
                            else
                            {
                                tempCondition += exp.ConditionList[j] + Constant.AND;
                            }
                        }
                        else
                        {
                            tempCondition += GetNotExpression(exp.ConditionList[i]);
                        }
                    }

                    //Replace child Guid
                    if (!string.IsNullOrEmpty(parentCondition) && parentCondition.IndexOf(exp.ID) > -1)
                    {
                        tempCondition = parentCondition.Replace(exp.ID, "(" + tempCondition + ")");
                    }

                    //if it's a expression that have child element, it will expand the child element
                    if (exp.ConditionList[i].IndexOf(Constant.CHILDFLG) > -1)
                    {
                        GetPatterns(exp.ChildExpressionDTO.First(o => o.ID == exp.ConditionList[i].Trim()), results, tempCondition);
                    }
                    else
                    {
                        results.Add(tempCondition);
                    }
                }
            }
        }

        public static string GetIfTemplate(string condition) 
        {
            string script = string.Empty;
            script += "if(" + condition + ")\n";
            script += "{\n";
            script += "}\n";

            return script;
        }
    }
}
