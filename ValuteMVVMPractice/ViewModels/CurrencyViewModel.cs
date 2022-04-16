using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ValuteMVVMPractice.Models;
using static ValuteMVVMPractice.Models.Currency;

namespace ValuteMVVMPractice.ViewModels
{
    public class ValCursViewModel : INotifyPropertyChanged
    {
        private ValCurs _valCurs;


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ValCursViewModel()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ValCurs));
            _valCurs = (ValCurs)xmlSerializer.Deserialize(new StreamReader("XML_daily.xml", Encoding.Default));
            Valute = _valCurs.Valute;
            Date = Convert.ToDateTime(_valCurs.Date);
            name = _valCurs.name;
        }

        public ValCursValute[] Valute
        {
            get { return _valCurs.Valute; }
            set
            {
                _valCurs.Valute = value;
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

     
    }

    public class ValCursValuteViewModel : INotifyPropertyChanged
    {
        private ValCursValute _valCursValute;

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ValCursValuteViewModel()
        {

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
