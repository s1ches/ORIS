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
    public  static readonly Dictionary<string, Func<IComparable?, IComparable?, bool>> BoolExpressionDictionary;

    static TemplateRegexes()
    {
        PropertyTemplateRegex = new (@"@{\w*}");
        IfTemplateRegex = new (@"@{if\(.*\)}");
        ThenTemplateRegex = new (@"@then{.\w*}");
        ElseTemplateRegex = new (@"@else{\w*}");
        FullIfTemplateRegex = new (@"@{if(.*)}");
        FullForTemplateRegex = new ("@for(.*){.*}");
        ForTemplateRegex = new (@"@for\(.*\)");
        ForPropertyTemplate = new (@"@{\w*.\w*}");
        BoolExpressionDictionary = new ()
        {
            { ">", (x,y) => x?.CompareTo(y) > 0},
            {"<", (x,y) => x?.CompareTo(y) < 0},
            {"<=", (x,y) => x?.CompareTo(y) <= 0},
            {">=", (x,y) => x?.CompareTo(y) >= 0},
            {"==", (x,y) => x?.CompareTo(y) == 0}
        };
    }
}