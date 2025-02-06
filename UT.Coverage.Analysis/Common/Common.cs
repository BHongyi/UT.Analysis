using UT.Coverage.Analysis.Dto;

namespace UT.Coverage.Analysis.Common
{
    public static class Common
    {
        public static string HandleExpression(string expression, ExpressionDTO exp)
        {
            exp.ID = Constant.CHILDFLG + Guid.NewGuid().ToString();
            exp.CondtionStr = expression;
            if (expression.IndexOf("|") > -1 && expression.IndexOf("&") > -1 && expression.IndexOf("|") < expression.IndexOf("&"))
            {
                exp.JudgeFlg = Constant.OR;
            }
            else if (expression.IndexOf("|") > -1 && expression.IndexOf("&") > -1 && expression.IndexOf("|") > expression.IndexOf("&"))
            {
                exp.JudgeFlg = Constant.AND;
            }
            else if (expression.IndexOf("|") > -1 && expression.IndexOf("&") == -1)
            {
                exp.JudgeFlg = Constant.OR;
            }
            else if (expression.IndexOf("|") == -1 && expression.IndexOf("&") > -1)
            {
                exp.JudgeFlg = Constant.AND;
            }

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

            exp.CondtionStr = RestoreParentheses(exp.CondtionStr);

            exp.ConditionList = exp.CondtionStr.Split(exp.JudgeFlg).ToList();

            return exp.ID;
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
