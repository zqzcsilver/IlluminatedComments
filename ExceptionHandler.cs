using System;

namespace IlluminatedComments
{
    internal static class ExceptionHandler
    {
        public static void Notify(Exception ex, bool showMessage)
        {
            var message = $"{DateTime.Now}: {ex}";
            Console.WriteLine(message);
            if (showMessage) UIMessage.Show(message);
        }
    }
}