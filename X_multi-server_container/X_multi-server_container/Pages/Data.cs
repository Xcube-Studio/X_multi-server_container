using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace X_multi_server_container.Pages
{

    public static class Data
    {
        public static ObservableCollection<HistoryModel> _historyList = null;
         public static ObservableCollection<HistoryModel> HistoryList
        {
            get
            {
                if (_historyList == null) { ReadHistoryModel(); SaveHistoryModel(); }
                return _historyList;
            }
            //set { _historyList = value; }
        }
        public static void HistoryListRemove(int i)
        {
            if (_historyList == null) { ReadHistoryModel(); }
            _historyList.RemoveAt(i);
            SaveHistoryModel();
        }
        public static void HistoryListAdd(HistoryModel model)
        {
            if (_historyList == null) { ReadHistoryModel(); }
            var addhead = new List<HistoryModel>();
            addhead.Add(model);
            addhead.AddRange(_historyList.ToList());
            _historyList.Clear();
            addhead.ForEach(l => _historyList.Add(l));
            SaveHistoryModel();
        }
        private static void ReadHistoryModel()
        {
            JArray historyJArr = new JArray();
            try
            {
                historyJArr = JArray.Parse(File.ReadAllText(Environment.CurrentDirectory + "\\history.json"));
            }
            catch (Exception) { }
            _historyList = new ObservableCollection<HistoryModel>();
            //_historyList.CollectionChanged += _historyList_CollectionChanged;
            historyJArr.ToList().ForEach(l =>
            {
                try
                {
                    string path = l.Value<string>("Path");
                    _historyList.Add(new HistoryModel(Path.GetFileName(path), Path.GetDirectoryName(path)));
                }
                catch (Exception) { }
            });
        }

        //private static void _historyList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    //throw new NotImplementedException();
        //}

        private static void SaveHistoryModel()
        {
            JArray historyJArr = new JArray();
            _historyList.ToList().ForEach(l => historyJArr.Add(new JObject { new JProperty("Path", l.subtitle + "\\" + l.title) }));
            File.WriteAllText(Environment.CurrentDirectory + "\\history.json", historyJArr.ToString());
        }
    }
}
