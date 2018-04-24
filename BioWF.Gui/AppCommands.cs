using System.Windows.Input;

namespace BioWF
{
    public static class AppCommands
    {
        public static readonly RoutedCommand Exit = new RoutedCommand("Exit", typeof(AppCommands));
    }
}