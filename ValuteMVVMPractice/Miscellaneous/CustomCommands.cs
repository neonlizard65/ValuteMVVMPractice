using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ValuteMVVMPractice.Miscellaneous
{
    public static class CustomCommands
    {
        public static RoutedCommand ShowMessage { get; set; }
        public static RoutedCommand MoveToCurrencyTable { get; set; }
        public static RoutedCommand MoveToGraph { get; set; }
        public static RoutedCommand MoveToConverter { get; set; }
        public static RoutedCommand MoveToSpravka{ get; set; }


        static CustomCommands()
        {
            InputGestureCollection inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.D1, ModifierKeys.Control, "Ctrl+1"));
            MoveToCurrencyTable = new RoutedCommand("MoveToCurrencyTable", typeof(CustomCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.D1, ModifierKeys.Control, "Ctrl+2"));
            MoveToGraph = new RoutedCommand("MoveToGraph", typeof(CustomCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.D1, ModifierKeys.Control, "Ctrl+3"));
            MoveToConverter = new RoutedCommand("MoveToConverter", typeof(CustomCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.D1, ModifierKeys.Control, "Ctrl+4"));
            MoveToSpravka = new RoutedCommand("MoveToSpravka", typeof(CustomCommands), inputs);
        }

        

    }
}
