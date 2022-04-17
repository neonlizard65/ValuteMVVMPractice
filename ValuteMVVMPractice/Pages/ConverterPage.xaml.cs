using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using ValuteMVVMPractice.ViewModels;

namespace ValuteMVVMPractice.Pages
{
    /// <summary>
    /// Interaction logic for ConverterPage.xaml
    /// </summary>
    public partial class ConverterPage : Page
    {
        public ConverterPage()
        {
            InitializeComponent();
            var Source = MainWindow.cvm.Valute.ToList();
            Source.Add(new ValCursValuteViewModel(643, "RUB", 1, "Российский рубль", 1, ""));
            Source = Source.OrderBy(x => x.Name).ToList();
            ValuteBox1.ItemsSource = Source;
            ValuteBox2.ItemsSource = Source;
        }

        private void Quantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9,]+$");
            bool r1 = regex.IsMatch(e.Text);
            bool r2 = true;
            if(e.Text == ",")
            {
                Regex regexAll = new Regex(@"^(\d+[,]?\d*)$");
                r2 = regexAll.IsMatch((sender as TextBox).Text + e.Text);
            }
            e.Handled = !(r1 && r2);
        }

        private void ValuteBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConvertVal();
        }

        private void Quantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            ConvertVal();
        }

        private void ConvertVal()
        {
            if (ValuteBox1.SelectedValue != null && ValuteBox2.SelectedValue != null)
            {
                decimal quantity = 0;
                if(Quantity.Text != "")
                {
                    quantity = decimal.Parse(Quantity.Text);
                }
                var answer = MainWindow.cvm.GetConversion(quantity, ValuteBox1.SelectedItem as ValCursValuteViewModel, ValuteBox2.SelectedItem as ValCursValuteViewModel).ToString();
                AnswerBox.Text = String.Format("{0:F6} " + (ValuteBox2.SelectedItem as ValCursValuteViewModel).CharCode, decimal.Parse(answer));
            }
        }

    }
}
