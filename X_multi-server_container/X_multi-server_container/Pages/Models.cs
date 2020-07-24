using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;

namespace X_multi_server_container.Pages
{
    public class HistoryModel : INotifyPropertyChanged
    {
        public HistoryModel(string title_, string subtitile_) /*,JObject StartINFO_*/
        {
            _title = title_;
            _subtitle = subtitile_;
            //_StartINFO = StartINFO_;
            _uuid = Guid.NewGuid().ToString();
        }
        private string _title;
        public string title
        {
            get { return _title; }
            set { _title = value; FirePropertyChanged("title"); }
        }
        private string _uuid;
        public string uuid
        {
            get { return _uuid; }
            set { _uuid = value; FirePropertyChanged("uuid"); }
        }
        private string _subtitle;
        public string subtitle
        {
            get { return _subtitle; }
            set { _subtitle = value; FirePropertyChanged("subtitle"); }
        }
        //private JObject _StartINFO;
        //public JObject StartINFO
        //{
        //    get { return _StartINFO; }
        //    set { _StartINFO = value; FirePropertyChanged("StartINFO"); }
        //}
        public virtual event PropertyChangedEventHandler PropertyChanged;
        public virtual void FirePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}