namespace MyTemplates;

public class Worker
{
    public string Ex1(string? name) => "Здравствуйте, @{name}, вы отчислены".Replace("@{name}", name);
    
    public string Ex2(object? obj)
    {
        var template = "Здравствуйте, @{name} вы прописаны по адресу @{address}";

        return template.Substitute(obj);
    }

    public string Ex3(object? obj)
    {
        var template = "Здравствуйте, @{if(temperature >= 37)} @then{Выздоравливайте} @else{Прогульщица}";
        
        return template.Substitute(obj);
    }

    public string Ex4(object? obj)
    {
        var template = "Здравствуйте, студенты группы @{group}. \nВаши баллы по ОРИС:\n @for(item in students) {@{item.FIO} баллы: @{item.grade}}";

        return template.Substitute(obj);
    }
}