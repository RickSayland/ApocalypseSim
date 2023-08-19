using ApocalypseSim;

Console.WriteLine("Good morning Rick! Today is {0}.",DateTime.Today.Date);

Console.Write("Generating todays scenario.");

var task = GPT4.Instance.Query("Please provide today's apocalypse scenario. Keep it short and sweet. Outline the major pain points I need to deal with today, and a to do list.");

while (!task.IsCompleted)
{
    Console.Write(".");
    Thread.Sleep(3000);
}
Console.WriteLine();
Console.WriteLine(task.Result);