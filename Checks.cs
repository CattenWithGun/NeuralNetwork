namespace ErrorChecks
{
    public static class Checks
    {
        public static bool NetworkListContainsName(params object[] args)
        {
            string name = args[0] as string ?? throw new NullReferenceException();
            List<string> networkNames = args[1] as List<string> ?? throw new NullReferenceException();
            return networkNames.Contains(name);
        }

        public static bool IsValidNetworkName(params object[] args)
        {
            string name = args[0] as string ?? throw new NullReferenceException();
            return !(name.Length > 20 || name.Length == 0 || NetworkListContainsName(args));
        }

        public static bool IsOption(params object[] args)
        {
            string option = args[0] as string ?? throw new NullReferenceException();
            //Goes through the list of commands, and if the option is in there, return that it is a command
            string[] commands = { "show", "train", "test", "error", "store", "make", "delete", "clear", "help" };
            for(int i = 0; i < commands.Length; i++)
            {
                if(commands[i] == option)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsYesNo(params object[] args)
        {
            string message = args[0] as string ?? throw new NullReferenceException();
            if(message != "y" && message != "n")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsDouble(params object[] args)
        {
            string doubleString = args[0] as string ?? throw new NullReferenceException();
            try
            {
                Convert.ToDouble(doubleString);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
