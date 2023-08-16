using System.Text;
using TopsisDemo;

var phones = new Phone[]
{
    new("Mobile 1", 250, 16, 12, Looks.Excellent),
    new("Mobile 2", 200, 16, 8, Looks.Average),
    new("Mobile 3", 300, 32, 16, Looks.Good),
    new("Mobile 4", 275, 32, 8, Looks.Good),
    new("Mobile 5", 225, 16, 16, Looks.BelowAverage),
};


Console.WriteLine("Considering all criteria equally");
var ranked = new TopsisBuilder<Phone>()
    .AddCriteria(p => p.Price, 0.25, Positivity.Negative)
    .AddCriteria(p => p.Storage, 0.25, Positivity.Positive)
    .AddCriteria(p => p.Camera, 0.25, Positivity.Positive)
    .AddCriteria(p => p.Appearance, 0.25, Positivity.Positive)
    .Execute(phones);

PrintTable(ranked);


Console.WriteLine("\n\nConsidering price at 70%");
var ranked1 = new TopsisBuilder<Phone>()
    .AddCriteria(p => p.Price, 0.7, Positivity.Negative)
    .AddCriteria(p => p.Storage, 0.1, Positivity.Positive)
    .AddCriteria(p => p.Camera, 0.1, Positivity.Positive)
    .AddCriteria(p => p.Appearance, 0.1, Positivity.Positive)
    .Execute(phones);

PrintTable(ranked1);




void PrintTable(TopsisResult<Phone>[] results)
{
    var nameWidth = Math.Max(results.Select(r => r.Item.Name.Length).Max(), "Name".Length);
    var priceWidth = Math.Max(results.Select(r => r.Item.Price.ToString().Length).Max(), "Price".Length);
    var storageWidth = Math.Max(results.Select(r => r.Item.Storage.ToString().Length).Max(), "Storage".Length);
    var cameraWidth = Math.Max(results.Select(r => r.Item.Camera.ToString().Length).Max(), "Camera".Length); 
    var looksWidth = Math.Max(results.Select(r => r.Item.Appearance.ToString().Length).Max(), "Looks".Length);
    var scoreWidth = Math.Max(results.Select(r => r.Score.ToString().Length).Max(), "Score".Length);

    var rowBuilder = new StringBuilder()
        .AppendFormat("Name".PadRight(nameWidth))
        .Append("    ")
        .AppendFormat("Price".PadRight(priceWidth))
        .Append("    ")
        .AppendFormat("Storage".PadRight(storageWidth))
        .Append("    ")
        .AppendFormat("Camera".PadRight(cameraWidth))
        .Append("    ")
        .AppendFormat("Looks".PadRight(looksWidth))
        .Append("    ")
        .AppendFormat("Score".PadRight(scoreWidth));
    Console.WriteLine(rowBuilder.ToString());
    
    foreach (var result in results)
    {
        rowBuilder = new StringBuilder()
            .AppendFormat(result.Item.Name.PadRight(nameWidth))
            .Append("    ")
            .AppendFormat(result.Item.Price.ToString().PadRight(priceWidth))
            .Append("    ")
            .AppendFormat(result.Item.Storage.ToString().PadRight(storageWidth))
            .Append("    ")
            .AppendFormat(result.Item.Camera.ToString().PadRight(cameraWidth))
            .Append("    ")
            .AppendFormat(result.Item.Appearance.ToString().PadRight(looksWidth))
            .Append("    ")
            .AppendFormat(result.Score.ToString().PadRight(scoreWidth));
        Console.WriteLine(rowBuilder.ToString());
    }

}


enum Looks
{
    Terrible = 1,
    BelowAverage,
    Average,
    Good,
    Excellent
}

record Phone(string Name, decimal Price, int Storage, int Camera, Looks Appearance);
