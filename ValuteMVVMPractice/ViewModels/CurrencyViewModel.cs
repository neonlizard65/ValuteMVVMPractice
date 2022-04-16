using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ValuteMVVMPractice.Models;
using static ValuteMVVMPractice.Models.Currency;

namespace ValuteMVVMPractice.ViewModels
{
    //ViewModel хранит данные, готовые к передаче на View. Будет создаваться объект этого класса.
    public class ValCursViewModel : INotifyPropertyChanged
    {   //Класс списка валют, используется для таблицы валют

        private ValCurs _valCurs;

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
                    Valute = _valCurs.Valute; //все валюты
                    name = _valCurs.name; //не используется
                }
            }
        }

        public ValCursViewModel()
        {   
            _valCurs = GetXML();
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

        private ValCurs GetXML()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ValCurs));
            HttpClient client = new HttpClient();
            var stream = client.GetAsync("https://cbr.ru/scripts/XML_daily.asp").Result.Content.ReadAsStreamAsync().Result;
            return (ValCurs)xmlSerializer.Deserialize(new StreamReader(stream, Encoding.Default));
        }

        private ValCurs GetXML(DateTime date)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ValCurs));
            HttpClient client = new HttpClient();
            var stream = client.GetAsync("https://cbr.ru/scripts/XML_daily.asp?date_req=" + String.Format("{0:dd/MM/yyyy}", date)).Result.Content.ReadAsStreamAsync().Result;
            return (ValCurs)xmlSerializer.Deserialize(new StreamReader(stream, Encoding.Default));
        }
    }


    //Создан на перспективу, но использоваться скорее всего не будет (нет смысла редактировать валюту индивидуально, а также все объекты уже в массиве класса выше)
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
