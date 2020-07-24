using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace X_multi_server_container.Pages
{
    public static class Data
    {
        private static List<HistoryModel> _historyList = null;
        public static List<HistoryModel> HistoryList
        {
            get { return _historyList; }
            set { _historyList = value; }
        }
        public static void HistoryListRemove(int i)
        {
            if (_historyList == null) { ReadHistoryModel(); }
            _historyList.RemoveAt(i);
        }
        public static void HistoryListAdd(HistoryModel model)
        {
            if (_historyList == null) { ReadHistoryModel(); }
            _historyList.Add(model);
        }
        private static void ReadHistoryModel()
        {
            var historyJArr = new JArray();
            try
            {
                historyJArr = JArray.Parse(File.ReadAllText(Environment.CurrentDirectory + "\\history.json"));
            }
            catch (Exception) { }


            //_historyList=
        }
        private static void SaveHistoryModel()
        {

        }
    }
}
