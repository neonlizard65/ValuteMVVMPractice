using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ValuteMVVMPractice.Pages;

namespace ValuteMVVMPractice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new CurrencyTablePage());
        }

        //Выход
        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены что хотите выйти?", "Выход", MessageBoxButton.OKCancel, MessageBoxImage.None);
            if(result == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
                
            }
        }
        private void Close_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        //Навигация Таблица
        private void MoveCurTableCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (MainFrame.Content.GetType() != typeof(CurrencyTablePage))
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
        private void MoveCurTableCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainFrame.Navigate(new CurrencyTablePage());
        }

        //Навигация График
        private void MoveToGraph_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (MainFrame.Content.GetType() != typeof(GraphPage))
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void MoveToGraph_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainFrame.Navigate(new GraphPage());
        }

        //Навигация Конвертер
        private void MoveToConverter_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (MainFrame.Content.GetType() != typeof(ConverterPage))
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void MoveToConverter_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainFrame.Navigate(new ConverterPage());
        }

        //Навигация Справка
        private void MoveToSpravka_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (MainFrame?.Content.GetType() != typeof(SpravkaPage))
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
            

        }

        private void MoveToSpravka_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainFrame.Navigate(new SpravkaPage());
        }
    }
}
