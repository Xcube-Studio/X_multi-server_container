using Newtonsoft.Json.Linq;

namespace X_multi_server_container.Pages
{
    internal class HistoryModel
    {
        private string v1;
        private string v2;
        private JObject jObjects;

        public HistoryModel(string v1, string v2, JObject jObjects)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.jObjects = jObjects;
        }
    }
}