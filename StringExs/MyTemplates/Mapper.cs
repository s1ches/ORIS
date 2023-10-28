using System.Reflection;
using System.Text.RegularExpressions;

namespace MyTemplates;

public class Mapper
{
    private readonly Regex _propertyTemplateRegex;
    private readonly Regex _ifTemplateRegex;
    private readonly Regex _thenTemplateRegex;
    private readonly Regex _elseTemplateRegex;
    private readonly Regex _fullIfTemplateRegex;
    private Dictionary<string, Func<IComparable?, IComparable?, bool>> _boolExpressionDictionary;

    public Mapper()
    {
        _propertyTemplateRegex = new Regex(@"@{\w*}");
        _ifTemplateRegex = new Regex(@"@{if\(.*\)}");
        _thenTemplateRegex = new Regex(@"@then{.\w*}");
        _elseTemplateRegex = new Regex(@"@else{\w*}");
        _fullIfTemplateRegex = new Regex(@"@{if(.*)}");
        _boolExpressionDictionary = new Dictionary<string, Func<IComparable?, IComparable?, bool>>()
        {
            { ">", (x,y) => x?.CompareTo(y) > 0},
            {"<", (x,y) => x?.CompareTo(y) < 0},
            {"<=", (x,y) => x?.CompareTo(y) <= 0},
            {">=", (x,y) => x?.CompareTo(y) >= 0},
            {"==", (x,y) => x?.CompareTo(y) == 0},
            {string.Empty, (x,y) => false}
        };
    }
    
    public string Ex1(string? name) => "Здравствуйте, @{name}, вы отчислены".Replace("@{name}", name);
    
    public string Ex2(object? obj)
    {
        if (obj is null) throw new NullReferenceException();
        
        var template = "Здравствуйте, @{name} вы прописаны по адресу @{address}";

        while (_propertyTemplateRegex.IsMatch(template))
        {
            var propertyTemplate = _propertyTemplateRegex.Match(template).Value;
            var propertyName = propertyTemplate.Substring(2, propertyTemplate.Length - 3);
            var propertyValue = obj.GetType().GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)?.GetValue(obj)?.ToString();
            template = template.Replace(propertyTemplate, propertyValue);
        }
        
        return template;
    }

    public string Ex3(object? obj)
    {
        if (obj is null) throw new NullReferenceException();

        var template = "Здравствуйте, @{if(temperature >= 37)} @then{Выздоравливайте} @else{Прогульщица}";

        try
        {
            var ifTemplate = _ifTemplateRegex.Match(template).Value.Substring(5);
            var thenValue = _thenTemplateRegex.Match(template).Value.Substring(6);
            var elseValue = _elseTemplateRegex.Match(template).Value.Substring(6);
            
            var fullIfTemplate = _fullIfTemplateRegex.Match(template).Value;

            var propertyName = new Regex(@"\w*").Match(ifTemplate).Value;
            
            var compareItem1 = (double?)obj.GetType()
                .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)?.GetValue(obj);
            
            var comparer = _boolExpressionDictionary.Keys.Where(x => ifTemplate.Contains(x)).Max() ?? "";
            var compareItem2 = double.Parse(new Regex(@"\d*").Matches(ifTemplate).Where(x => x.Value != "").FirstOrDefault()?.Value!);

            var ifExpressionResult = _boolExpressionDictionary[comparer](compareItem1, compareItem2);

            var resultValue = ifExpressionResult ? thenValue[..^1] : elseValue[..^1];
            template = template.Replace(fullIfTemplate, resultValue);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
        return template;

    }
}