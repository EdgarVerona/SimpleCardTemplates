using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCardTemplates.Engine.Templates
{
    public class Template
    {
        public string TemplateName { get; set; }
        public string TemplatePath { get; set; }

        public List<TemplateRegion> Regions { get; set; }

        public Template()
        {
            this.Regions = new List<TemplateRegion>();
        }

        public static List<Template> GetListFromFile(string file)
        {
            JsonSerializer serializer = JsonSerializer.Create();

            using (TextReader reader = File.OpenText(file))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(reader))
                {
                    return serializer.Deserialize<List<Template>>(jsonReader);
                }
            }
        }

    }
}
