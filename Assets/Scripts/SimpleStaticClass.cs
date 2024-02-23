using System;
using System.Security;

public static class SimpleStaticClass
{
    public static double SimpleStaticMethod()
    {
        return 5;
    }
    
    [SecurityCritical]
    internal static void SecurityCriticalMethod()
    {
        Console.WriteLine("This is a critical method.");
    }
}