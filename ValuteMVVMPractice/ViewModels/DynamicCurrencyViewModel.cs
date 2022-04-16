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
            }
        }

        public DynamicValCursViewModel(DateTime date1, DateTime date2, string code)
        {
            _valCurs = GetXML(date1.ToString(), date2.ToString(), code);
            Record = _valCurs.Record;
            ID = _valCurs.ID;
            DateRange1 = Convert.ToDateTime(_valCurs.DateRange1);
            DateRange2 = Convert.ToDateTime(_valCurs.DateRange2);
            name = _valCurs.name;
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
                _valCurs.ID = value;
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
                _valCurs.name = value.ToString();
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
                _valCurs.name = value.ToString();
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
                _valCurs.name = value;
            }
        }


        private ValCurs GetXML(string date1, string date2, string code)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ValCurs));
            HttpClient client = new HttpClient();
            var stream = client.GetAsync($"http://www.cbr.ru/scripts/XML_dynamic.asp?date_req1={String.Format("{0:dd/MM/yyyy}", date1)}&date_req2={String.Format("{ 0:dd / MM / yyyy", date1)}&VAL_NM_RQ={code}").Result.Content.ReadAsStreamAsync().Result;
            return (ValCurs)xs.Deserialize(stream);
        }
    }
}
