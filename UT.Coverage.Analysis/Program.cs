using UT.Coverage.Analysis.Common;
using UT.Coverage.Analysis.Dto;

string coveragePath = "../UT.TestPrject/coverage/UT.Project_App.xml";

//string expression = "if (i == 1 || i == 2 || (j == \"123\" && k == \"456\") || (k == \"789\" || (k == \"111\" && i == 3)))";

//string expression = "if (i == 1 || i == 2 || ((i == 4 && !string.IsNullOrEmpty(j)) && j == \"123\" && k == \"456\") || (k == \"789\" || (k == \"111\" && i == 3)))";

//string expression = "if (i == 1 && k.StartsWith(\"11\") && k.Length >= 2 && (j == \"3\" || j == \"4\"))";

string expression = "if (i == 1 && j.StartsWith(\"1\") && k.Length >= 2 && (k.Substring(1,1) == \"3\" || k.Substring(1, 1) == \"4\"))";

//string expression = "if (i == 1 || i == 2 || i == 3)";

//string expression = "if (i == 1 && i == 2 && i == 3)";

string handledExpression = expression.Substring(expression.IndexOf("(") + 1);

handledExpression = handledExpression.Substring(0, handledExpression.LastIndexOf(")"));

ExpressionDTO exp = new();
Common.HandleExpression(handledExpression,exp);

List<string> conditions = new List<string>();

Common.GetPatterns(exp, conditions);


foreach (string condition in conditions)
{
    Console.WriteLine(Common.GetIfTemplate(condition));
}

Console.ReadLine();