using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ValuteMVVMPractice.Interfaces;
using ValuteMVVMPractice.Models;
using static ValuteMVVMPractice.Models.Currency;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using System.Windows;

namespace ValuteMVVMPractice.ViewModels
{
    //ViewModel хранит данные, готовые к передаче на View. Будет создаваться объект этого класса.
    public class ValCursViewModel : INotifyPropertyChanged, IExportable
    {   //Класс списка валют, используется для таблицы валют

        private ValCurs _valCurs;
        private ValCursValute[] _valute;

        //Событие изменения свойства.
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                if (propertyName == "Date")
                {   //При изменении даты
                    _valCurs = GetXML(Date);
                    _valute = _valCurs.Valute;
                    ValCursValuteViewModel[] valutes = new ValCursValuteViewModel[_valute.Length];
                    for (int i = 0; i < _valute.Length; i++)
                    {
                        valutes[i] = new ValCursValuteViewModel(_valute[i]);
                    }
                    Valute = valutes; //все валюты
                    name = _valCurs.name; //не используется
                }
            }
        }

        //Конструктор
        public ValCursViewModel()
        {
            _valCurs = GetXML();
            _valute = _valCurs.Valute;
            ValCursValuteViewModel[] valutes = new ValCursValuteViewModel[_valute.Length];
            for (int i = 0; i < _valute.Length; i++)
            {
                valutes[i] = new ValCursValuteViewModel(_valute[i]);
            }
            Valute = valutes;
            Date = Convert.ToDateTime(_valCurs.Date);
            name = _valCurs.name;
        }

        //Свойства
        public ValCursValuteViewModel[] Valute
        {
            get
            {
                ValCursValuteViewModel[] valutes = new ValCursValuteViewModel[_valute.Length];
                for (int i = 0; i < _valute.Length; i++)
                {
                    valutes[i] = new ValCursValuteViewModel(_valute[i]);
                }
                return valutes;
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    _valute[i] = new ValCursValute
                    {
                        NumCode = value[i].NumCode,
                        CharCode = value[i].CharCode,
                        Nominal = value[i].Nominal,
                        Name = value[i].Name,
                        Value = value[i].Value.ToString(),
                        ID = value[i].ID
                    };

                }
                OnPropertyChanged("Valute");
            }
        }

        public DateTime Date
        {
            get { return Convert.ToDateTime(_valCurs.Date); }
            set
            {
                _valCurs.Date = value.ToString();
                OnPropertyChanged("Date");
            }
        }

        public string name
        {
            get { return _valCurs.name; }
            set
            {
                _valCurs.name = value;
                OnPropertyChanged("name");
            }
        }

        //Импорт
        private ValCurs GetXML()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ValCurs));
            HttpClient client = new HttpClient();
            var stream = client.GetStreamAsync("https://cbr.ru/scripts/XML_daily.asp").Result;
            return (ValCurs)xmlSerializer.Deserialize(new StreamReader(stream, Encoding.Default));
        }

        private ValCurs GetXML(DateTime date)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ValCurs));
            HttpClient client = new HttpClient();
            var stream = client.GetStreamAsync("https://cbr.ru/scripts/XML_daily.asp?date_req=" + String.Format("{0:dd/MM/yyyy}", date)).Result;
            return (ValCurs)xmlSerializer.Deserialize(new StreamReader(stream, Encoding.Default));
        }

        //Конвертация валют
        public decimal GetConversion(decimal quantity, ValCursValuteViewModel val1, ValCursValuteViewModel val2)
        {
            var answer = (Convert.ToDecimal(val1.Value) * quantity / val1.Nominal) / (Convert.ToDecimal(val2.Value));
            return (decimal)answer;
        }

        public void ExportToExcel()
        {
            try
            {
                Excel.Application excel = new Excel.Application();
                excel.SheetsInNewWorkbook = 1;
                Excel.Workbook workbook = excel.Workbooks.Add();
                Excel.Worksheet worksheet = excel.Worksheets[1];
                worksheet.Name = "Курсы валют";
                if (GetDataForExcel(ref worksheet))
                {
                    workbook.SaveAs(GetPath("xlsx"), Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, true, false, Excel.XlSaveAsAccessMode.xlNoChange,
                    Excel.XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, Type.Missing);
                    MessageBox.Show("Экспорт успешно выполнен");
                }

                workbook.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ExportToPDF()
        {
            try
            {
                var word = new Word.Application();
                Word.Document doc = word.Documents.Add();
                Word.Paragraph paragraph1 = doc.Paragraphs.Add();
                Word.Range rangep1 = paragraph1.Range;
                rangep1.Text = $"На дату: {Date:dd.MM.yyyy}";
                rangep1.Font.Bold = 1;
                rangep1.Font.Size = 36;
                Word.Paragraph paragraph2 = doc.Paragraphs.Add();
                Word.Range rangep2 = paragraph2.Range;
                rangep2.Font.Bold = 0;
                rangep2.Font.Size = 14;
                Word.Table table = doc.Tables.Add(rangep2, Valute.Length, 6);
                table.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitContent);
                table.Borders.InsideLineStyle = table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                if (GetDataForWord(ref table))
                {
                    doc.SaveAs(GetPath("pdf"), Word.WdSaveFormat.wdFormatPDF, false, System.Reflection.Missing.Value, false, System.Reflection.Missing.Value,
                        false, false, false, false, false);
                    MessageBox.Show("Экспорт успешно выполнен");
                }
                word.Quit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ExportToWord()
        {
            try
            {
                var word = new Word.Application();
                Word.Document doc = word.Documents.Add();
                Word.Paragraph paragraph1 = doc.Paragraphs.Add();
                Word.Range rangep1 = paragraph1.Range;
                rangep1.Text = $"На дату: {Date:dd.MM.yyyy}";
                rangep1.Font.Bold = 1;
                rangep1.Font.Size = 36;
                Word.Paragraph paragraph2 = doc.Paragraphs.Add();
                Word.Range rangep2 = paragraph2.Range;
                rangep2.Font.Bold = 0;
                rangep2.Font.Size = 14;
                Word.Table table = doc.Tables.Add(rangep2, Valute.Length, 6);
                table.AutoFitBehavior(Word.WdAutoFitBehavior.wdAutoFitContent);
                table.Borders.InsideLineStyle = table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
               
                if (GetDataForWord(ref table))
                {
                    doc.SaveAs(GetPath("docx"), Word.WdSaveFormat.wdFormatXMLDocument, false, System.Reflection.Missing.Value, false, System.Reflection.Missing.Value,
                        false, false, false, false, false);
                    MessageBox.Show("Экспорт успешно выполнен");
                }
                word.Quit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetPath(string format)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = $".{format} файлы (*.{format})|*.{format}|Все файлы (*.*)|*.*)";

            var result = saveDialog.ShowDialog();
            if ((bool)result && result != null)
            {
                return saveDialog.FileName;
            }
            return null;
        }

        private bool GetDataForExcel(ref Excel.Worksheet worksheet)
        {  
            try
            {
                worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, 6]].Merge();
                worksheet.Cells[1, 1] = $"На дату: {Date:dd.MM.yyyy}";
                worksheet.Cells[1, 1].Font.Bold = true;
                worksheet.Cells[1, 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                worksheet.Cells[2, 1].Value = "ID";
                worksheet.Cells[2, 2].Value = "Код";
                worksheet.Cells[2, 3].Value = "Название";
                worksheet.Cells[2, 4].Value = "Номинал";
                worksheet.Cells[2, 5].Value = "Курс";
                worksheet.Cells[2, 6].Value = "Курс к одной единице";
                for(int i = 1; i <= 6; i++)
                {
                    worksheet.Cells[2, i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                }


                for (int i = 3; i < Valute.Length + 3; i++)
                {
                    var val = Valute[i - 3];
                    worksheet.Cells[i, 1].Value = val.NumCode;
                    worksheet.Cells[i, 2].Value = val.CharCode;
                    worksheet.Cells[i, 3].Value = val.Name;
                    worksheet.Cells[i, 4].Value = val.Nominal;
                    worksheet.Cells[i, 4].NumberFormat = "0";
                    worksheet.Cells[i, 5].Value = val.Value;
                    worksheet.Cells[i, 5].NumberFormat = "0,0000";
                    worksheet.Cells[i, 6].Formula = $"=E{i}/D{i}";
                    worksheet.Cells[i, 6].NumberFormat = "0,000000";
                }
                Excel.Range rangeBorders = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[Valute.Length + 2, 6]];
                rangeBorders.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                worksheet.Columns.AutoFit();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }


        }

        private bool GetDataForWord(ref Word.Table table)
        {
            try
            {
                Word.Range cell;
                cell = table.Cell(1, 1).Range;
                cell.Text = "ID";
                cell = table.Cell(1, 2).Range;
                cell.Text = "Код";
                cell = table.Cell(1, 3).Range;
                cell.Text = "Название";
                cell = table.Cell(1, 4).Range;
                cell.Text = "Номинал";
                cell = table.Cell(1, 5).Range;
                cell.Text = "Курс";
                cell = table.Cell(1, 6).Range;
                cell.Text = "Курс к одной единице";

                table.Rows[1].Range.Bold = 1;
                table.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                for (int i = 2; i < Valute.Length + 2; i++)
                {
                    var val = Valute[i - 2];
                    cell = table.Cell(i, 1).Range;
                    cell.Text = val.NumCode.ToString();
                    cell = table.Cell(i, 2).Range;
                    cell.Text = val.CharCode.ToString();
                    cell = table.Cell(i, 3).Range;
                    cell.Text = val.Name.ToString();
                    cell = table.Cell(i, 4).Range;
                    cell.Text = val.Nominal.ToString();
                    cell = table.Cell(i, 5).Range;
                    cell.Text = String.Format("{0:F4}", val.Value.ToString());
                    cell = table.Cell(i, 6).Range;
                    cell.Text = String.Format("{0:F6}", (val.Value/val.Nominal).ToString());
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }

    public class ValCursValuteViewModel : INotifyPropertyChanged
    {
        private ValCursValute _valCursValute;

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ValCursValuteViewModel(ValCursValute val)
        {   //Обычный конструкор
            _valCursValute = val;
            this.NumCode = val.NumCode;
            this.CharCode = val.CharCode;
            this.Nominal = val.Nominal;
            this.Name = val.Name;
            this.Value = decimal.Parse(val.Value);
            this.ID = val.ID;
        }

        public ValCursValuteViewModel(ushort NumCode, string CharCode, ushort Nominal, string Name, decimal Value, string ID)
        {   //Конструктор для добавления рубля для конвертации
            _valCursValute = new ValCursValute()
            {
                NumCode = NumCode,
                CharCode = CharCode,
                Nominal = Nominal,
                Name = Name,
                Value = Value.ToString(),
                ID = ID
            };
            this.NumCode = NumCode;
            this.CharCode = CharCode;
            this.Nominal = Nominal;
            this.Name = Name;
            this.Value = Value;
            this.ID = ID;
        }

        public ushort NumCode
        {
            get
            {
                return _valCursValute.NumCode;
            }
            set
            {
                _valCursValute.NumCode = value;
                OnPropertyChanged("NumCode");
            }
        }

        public string CharCode
        {
            get
            {
                return (_valCursValute.CharCode);
            }
            set
            {
                _valCursValute.CharCode = value;
                OnPropertyChanged("CharCode");
            }
        }

        public ushort Nominal
        {
            get
            {
                return _valCursValute.Nominal;
            }
            set
            {
                _valCursValute.Nominal = value;
                OnPropertyChanged("Nominal");
            }
        }

        public string Name
        {
            get
            {
                return _valCursValute.Name;
            }
            set
            {
                _valCursValute.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public decimal Value
        {
            get
            {
                return decimal.Parse(_valCursValute.Value);
            }
            set
            {
                _valCursValute.Value = value.ToString();
                OnPropertyChanged("Value");
            }
        }

        public string ID
        {
            get
            {
                return _valCursValute.ID;
            }
            set
            {
                _valCursValute.ID = value;
                OnPropertyChanged("ID");
            }
        }


    }
}


