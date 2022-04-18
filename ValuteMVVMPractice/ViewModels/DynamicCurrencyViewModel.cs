using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
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
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using System.Windows;

using static ValuteMVVMPractice.Models.DynamicCurrency;
using static ValuteMVVMPractice.Models.Currency;
using ValuteMVVMPractice.Models;

namespace ValuteMVVMPractice.ViewModels
{
    public class DynamicValCursViewModel : INotifyPropertyChanged, IExportable
    {

        private DynamicCurrency.ValCurs _valCurs;
        private ValCursValuteViewModel _valute;

        //Обработчик при изменении
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                if (propertyName == "ID")
                {
                    _valCurs = GetXML(_valCurs.DateRange1.ToString(), _valCurs.DateRange2.ToString(), _valCurs.ID);
                    _valute = new ValCursViewModel().Valute.Where(x => x.ID == ID).FirstOrDefault();
                    Record = _valCurs.Record;
                    DateRange1 = Convert.ToDateTime(_valCurs.DateRange1);
                    DateRange2 = Convert.ToDateTime(_valCurs.DateRange2);
                    name = _valCurs.name;
                }
                else if (propertyName == "DateRange1")
                {
                    _valCurs = GetXML(_valCurs.DateRange1.ToString(), _valCurs.DateRange2.ToString(), _valCurs.ID);
                    _valute = new ValCursViewModel().Valute.Where(x => x.ID == ID).FirstOrDefault();
                    Record = _valCurs.Record;
                    ID = _valCurs.ID;
                    name = _valCurs.name;
                }
                else if (propertyName == "DateRange2")
                {
                    _valCurs = GetXML(_valCurs.DateRange1.ToString(), _valCurs.DateRange2.ToString(), _valCurs.ID);
                    _valute = new ValCursViewModel().Valute.Where(x => x.ID == ID).FirstOrDefault();
                    Record = _valCurs.Record;
                    ID = _valCurs.ID;
                    name = _valCurs.name;
                }
            }
            plotModel = GetPlot();
        }

        //Конструктор
        public DynamicValCursViewModel(DateTime date1, DateTime date2, string code)
        {
            
            _valCurs = GetXML(date1.ToString(), date2.ToString(), code);
            _valute = new ValCursViewModel().Valute.Where(x => x.ID == ID).FirstOrDefault();
            Record = _valCurs.Record;
            ID = _valCurs.ID;
            DateRange1 = Convert.ToDateTime(_valCurs.DateRange1);
            DateRange2 = Convert.ToDateTime(_valCurs.DateRange2);
            name = _valCurs.name;
            plotModel = GetPlot();
        }

       //Свойства
        public ValCursRecord[] Record 
        { 
            get 
            { 
                return _valCurs.Record; 
            } 
            set 
            {
                _valCurs.Record = value;
            } 
        }
        public string ID
        {
            get
            {
                return _valCurs.ID;
            }
            set
            {
                if (_valCurs?.ID != value.ToString())
                {
                    _valCurs.ID = value;
                    OnPropertyChanged("ID");
                }
            }
        }
        public DateTime DateRange1
        {
            get
            {
                return Convert.ToDateTime(_valCurs.DateRange1);
            }
            set
            {
                if (Convert.ToDateTime(_valCurs?.DateRange1) != value && value < Convert.ToDateTime(_valCurs?.DateRange2))
                {
                    _valCurs.DateRange1 = value.ToString();
                    OnPropertyChanged("DateRange1");
                }
            }
        }
        public DateTime DateRange2
        {
            get
            {
                return Convert.ToDateTime(_valCurs.DateRange2);
            }
            set
            {
                if (Convert.ToDateTime(_valCurs?.DateRange2) != value && value > Convert.ToDateTime(_valCurs?.DateRange1))
                {
                    _valCurs.DateRange2 = value.ToString();
                    OnPropertyChanged("DateRange2");
                }
            }
        }
        public string name
        {
            get
            {
                return _valCurs.name;
            }
            set
            {
                if(_valCurs.name != value)
                {
                    _valCurs.name = value;
                    OnPropertyChanged("name");
                }
            
            }
        }

        //Импорт
        private DynamicCurrency.ValCurs GetXML(string date1, string date2, string code)
        {
            XmlSerializer xs = new XmlSerializer(typeof(DynamicCurrency.ValCurs));
            HttpClient client = new HttpClient();
            string link = "http://www.cbr.ru/scripts/XML_dynamic.asp?date_req1=" + String.Format("{0:dd/MM/yyyy}", date1) + "&date_req2=" + String.Format("{0:dd/MM/yyyy}", date2) + "&VAL_NM_RQ=" + code;
            var stream = client.GetStreamAsync(link).Result;
            return (DynamicCurrency.ValCurs)xs.Deserialize(stream);
        }


        //График
        private PlotModel plotModel;
        public PlotModel GetPlot()
        {
            //series
            var line1 = new OxyPlot.Series.LineSeries()
            {
                Title = name,
                Color = OxyPlot.OxyColors.Blue,
                StrokeThickness = 1
            };


            //x
            var minValue = DateTimeAxis.ToDouble(DateRange1);
            var maxValue = DateTimeAxis.ToDouble(DateRange2);
            var ax = new DateTimeAxis()
            {
                Minimum = minValue,
                Maximum = maxValue,
                StringFormat = "yyyy-MM-dd",
                Position = AxisPosition.Bottom,
                Angle = 45,
                IsZoomEnabled = true
            };

            double miny = 1000000000;
            double maxy = 0;
            foreach (var record in Record)
            {
                var x = DateTimeAxis.ToDouble(Convert.ToDateTime(record.Date));
                var y = Convert.ToDouble(record.Value);
                if(y == 0)
                {
                    continue;
                }
                line1.Points.Add(new OxyPlot.DataPoint(x, y));
                if (y > maxy) { maxy = y; }
                if(y < miny) { miny = y; }
            }

            //y
            var ay = new LinearAxis()
            {
                Key = "y",
                Position = AxisPosition.Left,
                Minimum = miny - 2,
                Maximum = maxy + 2,
                MajorStep = Math.Round(((maxy + 2) - (miny- 2)) / 5)
            };

            plotModel = new OxyPlot.PlotModel
            {
                Title = _valute.Name
            };
            plotModel.Axes.Add(ay);
            plotModel.Axes.Add(ax);
            plotModel.Series.Add(line1);


            return plotModel;
        }

        //Экспорт
        public void ExportToExcel()
        {
            try
            {
                Excel.Application excel = new Excel.Application();
                excel.SheetsInNewWorkbook = 1;
                Excel.Workbook workbook = excel.Workbooks.Add();
                Excel.Worksheet worksheet = excel.Worksheets[1];
                worksheet.Name = "Динамика валюты";
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

        public void ExportToWord()
        {
            try
            {
                var word = new Word.Application();
                Word.Document doc = word.Documents.Add();
                Word.Paragraph paragraph1 = doc.Paragraphs.Add();
                Word.Range rangep1 = paragraph1.Range;
                rangep1.Text = $"Период: {DateRange1:dd.MM.yyyy} - {DateRange2:dd.MM.yyyy}";
                rangep1.Font.Bold = 1;
                rangep1.Font.Size = 24;
                Word.Paragraph paragraph2 = doc.Paragraphs.Add();
                Word.Range rangep2 = paragraph2.Range;
                rangep2.Text = $"{_valute.Name} ({_valute.CharCode})";
                rangep2.Font.Bold = 1;
                rangep2.Font.Size = 24;
                rangep2.Font.Italic = 1;
                Word.Paragraph paragraph3 = doc.Paragraphs.Add();
                Word.Range rangep3 = paragraph3.Range;
                rangep3.Font.Bold = 0;
                rangep3.Font.Size = 14;
                rangep3.Font.Italic = 0;
                Word.Table table = doc.Tables.Add(rangep3, Record.Length, 4);
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
        public void ExportToPDF()
        {
            using (var stream = File.Create(GetPath("pdf")))
            {
                var pdfExporter = new OxyPlot.SkiaSharp.PdfExporter { Width = 600, Height = 400, UseTextShaping = true };
                pdfExporter.Export(plotModel, stream);
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
                worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, 4]].Merge();
                worksheet.Range[worksheet.Cells[2, 1], worksheet.Cells[2, 4]].Merge();
                worksheet.Cells[1, 1] = $"Период: {DateRange1:dd.MM.yyyy} - {DateRange2:dd.MM.yyyy}";
                worksheet.Cells[1, 1].Font.Bold = true;
                worksheet.Cells[1, 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                worksheet.Cells[2, 1].Value = $"{_valute.Name} ({_valute.CharCode})";
                worksheet.Cells[2, 1].Font.Bold = true;
                worksheet.Cells[2, 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                worksheet.Cells[3, 1].Value = "Дата";
                worksheet.Cells[3, 2].Value = "Номинал";
                worksheet.Cells[3, 3].Value = "Курс";
                worksheet.Cells[3, 4].Value = "Курс к одной единице";
                for (int i = 1; i <= 6; i++)
                {
                    worksheet.Cells[2, i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                }


                for (int i = 4; i < Record.Length + 4; i++)
                {
                    var val = Record[i - 4];
                    worksheet.Cells[i, 1].Value = String.Format("{0:dd.MM.yyyy}", Convert.ToDateTime(val.Date));
                    worksheet.Cells[i, 2].Value = val.Nominal;
                    worksheet.Cells[i, 2].NumberFormat = "0";
                    worksheet.Cells[i, 3].Value = decimal.Parse(val.Value);
                    worksheet.Cells[i, 3].NumberFormat = "0,0000";
                    worksheet.Cells[i, 4].Formula = $"=C{i}/B{i}";
                    worksheet.Cells[i, 4].NumberFormat = "0,000000";
                }
                Excel.Range rangeBorders = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[Record.Length + 3, 4]];
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
                cell.Text = "Дата";
                cell = table.Cell(1, 2).Range;
                cell.Text = "Номинал";
                cell = table.Cell(1, 3).Range;
                cell.Text = "Курс";
                cell = table.Cell(1, 4).Range;
                cell.Text = "Курс к одной единице";

                table.Rows[1].Range.Bold = 1;
                table.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                for (int i = 2; i < Record.Length + 2; i++)
                {
                    var val = Record[i - 2];
                    cell = table.Cell(i, 1).Range;
                    cell.Text = val.Date.ToString();
                    cell = table.Cell(i, 2).Range;
                    cell.Text = val.Nominal.ToString();
                    cell = table.Cell(i, 3).Range;
                    cell.Text = String.Format("{0:F4}", val.Value.ToString());
                    cell = table.Cell(i, 4).Range;
                    cell.Text = String.Format("{0:F6}", (decimal.Parse(val.Value) / val.Nominal).ToString());
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
}
