using System.Text.RegularExpressions;

namespace MyTemplates;

public static class TemplateRegexes
{
    public static readonly Regex PropertyTemplateRegex;
    public static readonly Regex IfTemplateRegex;
    public static readonly Regex ThenTemplateRegex;
    public static readonly Regex ElseTemplateRegex;
    public static readonly Regex FullIfTemplateRegex;
    public static readonly Regex FullForTemplateRegex;
    public static readonly Regex ForTemplateRegex;
    public static readonly Regex ForPropertyTemplate;
    public  static Dictionary<string, Func<IComparable?, IComparable?, bool>> BoolExpressionDictionary;

    static TemplateRegexes()
    {
        PropertyTemplateRegex = new Regex(@"@{\w*}");
        IfTemplateRegex = new Regex(@"@{if\(.*\)}");
        ThenTemplateRegex = new Regex(@"@then{.\w*}");
        ElseTemplateRegex = new Regex(@"@else{\w*}");
        FullIfTemplateRegex = new Regex(@"@{if(.*)}");
        FullForTemplateRegex = new Regex("@for(.*){.*}");
        ForTemplateRegex = new Regex(@"@for\(.*\)");
        ForPropertyTemplate = new Regex(@"@{\w*.\w*}");
        BoolExpressionDictionary = new Dictionary<string, Func<IComparable?, IComparable?, bool>>()
        {
            { ">", (x,y) => x?.CompareTo(y) > 0},
            {"<", (x,y) => x?.CompareTo(y) < 0},
            {"<=", (x,y) => x?.CompareTo(y) <= 0},
            {">=", (x,y) => x?.CompareTo(y) >= 0},
            {"==", (x,y) => x?.CompareTo(y) == 0}
        };
    }
}