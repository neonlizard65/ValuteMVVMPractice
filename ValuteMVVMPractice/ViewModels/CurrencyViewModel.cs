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
                    for(int i = 0; i < _valute.Length; i++)
                    {
                        valutes[i] = new ValCursValuteViewModel(_valute[i]);
                    }
                    Valute = valutes; //все валюты
                    name = _valCurs.name; //не используется
                }
            }
        }

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
                for(int i = 0; i < value.Length; i++)
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

        public decimal GetConversion(decimal quantity, ValCursValuteViewModel val1, ValCursValuteViewModel val2)
        {
            var answer = (Convert.ToDecimal(val1.Value) * quantity / val1.Nominal) / (Convert.ToDecimal(val2.Value));
            return (decimal)answer;
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
