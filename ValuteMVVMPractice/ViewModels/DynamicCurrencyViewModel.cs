using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static ValuteMVVMPractice.Models.DynamicCurrency;

namespace ValuteMVVMPractice.ViewModels
{
    public class DynamicValCursViewModel : INotifyPropertyChanged
    {

        private ValCurs _valCurs;

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
                    Record = _valCurs.Record;
                    DateRange1 = Convert.ToDateTime(_valCurs.DateRange1);
                    DateRange2 = Convert.ToDateTime(_valCurs.DateRange2);
                    name = _valCurs.name;
                }
                else if (propertyName == "DateRange1")
                {
                    _valCurs = GetXML(_valCurs.DateRange1.ToString(), _valCurs.DateRange2.ToString(), _valCurs.ID);
                    Record = _valCurs.Record;
                    ID = _valCurs.ID;
                    name = _valCurs.name;
                }
                else if (propertyName == "DateRange2")
                {
                    _valCurs = GetXML(_valCurs.DateRange1.ToString(), _valCurs.DateRange2.ToString(), _valCurs.ID);
                    Record = _valCurs.Record;
                    ID = _valCurs.ID;
                    name = _valCurs.name;
                }
            }
            plotModel = GetPlot();
        }

        public DynamicValCursViewModel(DateTime date1, DateTime date2, string code)
        {
            _valCurs = GetXML(date1.ToString(), date2.ToString(), code);
            Record = _valCurs.Record;
            ID = _valCurs.ID;
            DateRange1 = Convert.ToDateTime(_valCurs.DateRange1);
            DateRange2 = Convert.ToDateTime(_valCurs.DateRange2);
            name = _valCurs.name;
            plotModel = GetPlot();
        }

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



        private ValCurs GetXML(string date1, string date2, string code)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ValCurs));
            HttpClient client = new HttpClient();
            string link = "http://www.cbr.ru/scripts/XML_dynamic.asp?date_req1=" + String.Format("{0:dd/MM/yyyy}", date1) + "&date_req2=" + String.Format("{0:dd/MM/yyyy}", date2) + "&VAL_NM_RQ=" + code;
            var stream = client.GetStreamAsync(link).Result;
            return (ValCurs)xs.Deserialize(stream);
        }


        public PlotModel plotModel;
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
                Title = MainWindow.cvm.Valute.Where(x => x.ID == this.ID).FirstOrDefault().Name
            };
            plotModel.Axes.Add(ay);
            plotModel.Axes.Add(ax);
            plotModel.Series.Add(line1);


            return plotModel;
        }

    }
}
