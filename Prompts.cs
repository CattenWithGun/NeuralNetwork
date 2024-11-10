internal static class Prompts
{
    public delegate bool ConditionMethod(params object[] args);

    public static string PromptUntilConditionMet(string startingMessage, string problemMessage, ConditionMethod conditionMethod, params object[] args)
    {
        Console.Write(startingMessage);
        //If the input is null, default to exiting
        string option = Console.ReadLine() ?? "exit";
        if (option == "exit") return "exit";
        args[0] = option;
        while (!conditionMethod(args))
        {
            Console.Write(problemMessage);
            option = Console.ReadLine() ?? "exit";
            if (option == "exit") return "exit";
            args[0] = option;
        }
        return option;
    }

    public static string SamePromptUntilConditionMet(string message, ConditionMethod conditionMethod, params object[] args)
    {
        string option = "";
        args[0] = option;
        do
        {
            Console.Write(message);
            option = Console.ReadLine() ?? "exit";
            if (option == "exit") return "exit";
            args[0] = option;
        }
        while (!conditionMethod(args));
        return option;
    }
}
