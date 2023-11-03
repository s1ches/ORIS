using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MyTemplates;

public static class TemplateStringExtension
{
    public static string Substitute(this string template, object? obj)
    {
        if (obj is null) throw new ArgumentNullException();
        
        var result = template;
        
        while (TemplateRegexes.PropertyTemplateRegex.IsMatch(result))
            result = result.SubstituteProperty(obj);

        while (TemplateRegexes.FullIfTemplateRegex.IsMatch(result))
            result = result.SubstituteIfConstruction(obj);

        while (TemplateRegexes.FullForTemplateRegex.IsMatch(result))
            result = result.SubstituteForConstruction(obj);
        

        return result;
    }

    private static string SubstituteProperty(this string template, object obj)
    {
        var result = template;
        try
        {
            var propertyTemplate = TemplateRegexes.PropertyTemplateRegex.Match(result).Value;
            var propertyName = propertyTemplate[2..^1];
            var propertyValue = obj.GetType().GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)?.GetValue(obj)?.ToString();
            result = result.Replace(propertyTemplate, propertyValue);
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }

        return result;
    }
    
    private static string SubstituteIfConstruction(this string template, object obj)
    {
        var result = template;
        try
        {
            var ifTemplate = TemplateRegexes.IfTemplateRegex.Match(template).Value[5..];
            var thenValue = TemplateRegexes.ThenTemplateRegex.Match(template).Value[6..^1];
            var elseValue = TemplateRegexes.ElseTemplateRegex.Match(template).Value[6..^1];
            
            var fullIfTemplate = TemplateRegexes.FullIfTemplateRegex.Match(template).Value;

            var propertyName = new Regex(@"\w*").Match(ifTemplate).Value;
            
            var compareItem1 = (double?)obj.GetType()
                .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)?.GetValue(obj);
            
            var comparer = TemplateRegexes.BoolExpressionDictionary.Keys.Where(x => ifTemplate.Contains(x)).Max() ?? "";
            var compareItem2 = double.Parse(new Regex(@"\d*").Matches(ifTemplate).FirstOrDefault(x => x.Value != "")?.Value!);

            var ifExpressionResult = TemplateRegexes.BoolExpressionDictionary[comparer](compareItem1, compareItem2);

            var resultValue = ifExpressionResult ? thenValue : elseValue;
            result = template.Replace(fullIfTemplate, resultValue);
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }

        return result;
    }

    private static string SubstituteForConstruction(this string template, object obj)
    {
        var result = template;
        try
        {
            var fullForTemplate = TemplateRegexes.FullForTemplateRegex.Match(template).Value;
            var forTemplate = TemplateRegexes.ForTemplateRegex.Match(fullForTemplate).Value[1..^1];
            var forBodyTemplate = fullForTemplate[(forTemplate.Length+4)..^1];
            var forPropertiesTemplate = TemplateRegexes.ForPropertyTemplate.Matches(fullForTemplate).Select(x => x.Value).ToArray();
            var forCollectionTemplate = forTemplate.Split().Last();
            
            template = template.Replace(fullForTemplate, "");

            var templateBuilder = new StringBuilder(template);

            var collection = obj.GetType()
                .GetProperty(forCollectionTemplate, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)?
                .GetValue(obj) as IEnumerable<object> ?? new List<object>();

            var propertyNameRegex = new Regex(@"\..*");
            
            foreach (var item in collection)
            {
                string forBody = forBodyTemplate;
                foreach(var propertyTemplate in forPropertiesTemplate)
                {
                    var propertyName = propertyNameRegex.Match(propertyTemplate).Value[1..^1];
                    var propertyValue = item.GetType()
                        .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)?
                        .GetValue(item)?.ToString() ?? string.Empty;
                    
                    forBody = forBody.Replace(propertyTemplate, propertyValue);
                }

                templateBuilder.AppendFormat("{0}\n", forBody);
            }
            
            result = templateBuilder.ToString();
        }catch(Exception ex) { Console.WriteLine(ex.Message); }

        return result;
    }
}