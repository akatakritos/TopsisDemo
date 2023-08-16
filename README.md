# TOPSIS Demo in C#

TOPSIS is a weighted-ranking algorithm for when you have a multiple factors you'd like to rank by. 

Based on https://medium.com/analytics-vidhya/how-to-decide-between-multiple-equally-good-things-simple-use-math-7e517f1422d5

## Usage

For each criteria you want to rank on:

1. Convert it into some kind of number. This implementation takes ints and doubles and enums. Enums are converted to
   to their numeric value
2. Decide if higher values are better or worse. This is called the positivity. For example, a higher price is worse, so
   it's negative. A higher storage is better, so it's positive.
3. Decide how important this criteria is. This is called the weight. For example, price is more important than storage,
   so it's weight is higher.

Then use the `TopsisBuilder` to configure the algorithm. Add as many criteria as you want, specifying which property to
read from, the weight, and the positivity. Then execute it against your list of items.

You'll get back an array of ranked results, holding the item and its score. The score is a number between 0 and 1, where 1 is
the best possible score.

```csharp
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
```

```text
Considering all criteria equally
Name        Price    Storage    Camera    Looks           Score              
Mobile 3    300      32         16        Good            0.6916322312675315 
Mobile 4    275      32         8         Good            0.534736584486838  
Mobile 1    250      16         12        Excellent       0.5342768571821003 
Mobile 5    225      16         16        BelowAverage    0.40104612151678615
Mobile 2    200      16         8         Average         0.3083677687324685 


Considering price at 70%
Name        Price    Storage    Camera    Looks           Score             
Mobile 2    200      16         8         Average         0.7221493625641102
Mobile 5    225      16         16        BelowAverage    0.6339331234163232
Mobile 1    250      16         12        Excellent       0.5091539682402054
Mobile 4    275      32         8         Good            0.3351595378736587
```
            
