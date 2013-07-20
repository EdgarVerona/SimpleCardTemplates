using Newtonsoft.Json;
using SimpleCardTemplates.Engine.Templates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleCardTemplates.TemplateUtility
{
    public partial class frmTemplateEntry : Form
    {

        public List<Template> Templates = new List<Template>();

        public Template CurrentTemplate = null;
        public TemplateRegion CurrentRegion = null;

        public frmTemplateEntry()
        {
            InitializeComponent();

            cboTemplates.SelectedIndexChanged += cboTemplates_SelectedIndexChanged;
            cboRegion.SelectedIndexChanged += cboRegion_SelectedIndexChanged;
            chkBold.CheckedChanged += chkBold_CheckedChanged;
            chkItalic.CheckedChanged += chkItalic_CheckedChanged;
            chkCenter.CheckedChanged += chkCenter_CheckedChanged;
            chkInvert.CheckedChanged += chkInvert_CheckedChanged;
            templateRegionSelector.OnRegionChange += templateRegionSelector_OnRegionChange;

            this.Templates = Template.GetListFromFile(ConfigurationManager.AppSettings["TemplateFilePath"]);

            cboTemplates.Items.AddRange(this.Templates.Select(t => t.TemplateName).ToArray());

            SetCurrentTemplate(this.Templates[0]);
            SetCurrentRegion(this.Templates[0].Regions[0]);
        }


        void templateRegionSelector_OnRegionChange(object sender, TemplateRegion e)
        {
            SetCurrentRegion(e);
        }


        void chkInvert_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CurrentRegion != null)
            {
                this.CurrentRegion.Invert = chkInvert.Checked;
            }
        }

        void chkCenter_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CurrentRegion != null)
            {
                this.CurrentRegion.Center = chkCenter.Checked;
            }
        }

        void chkItalic_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CurrentRegion != null)
            {
                this.CurrentRegion.Italic = chkItalic.Checked;
            }
        }

        void chkBold_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CurrentRegion != null)
            {
                this.CurrentRegion.Bold = chkBold.Checked;
            }
        }

        void cboTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            var template = this.Templates.FirstOrDefault(t => t.TemplateName.Equals(cboTemplates.SelectedItem));

            if (template != null && this.CurrentTemplate != template)
            {
                SetCurrentTemplate(template);
                SetCurrentRegion(template.Regions[0]);
            }
        }


        void cboRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            var region = this.CurrentTemplate.Regions.FirstOrDefault(r => r.RegionName.Equals(cboRegion.SelectedItem));

            if (region != null && this.CurrentRegion != region)
            {
                SetCurrentRegion(region);
            }
        }


        private void SetCurrentTemplate(Template template)
        {
            templateRegionSelector.BoundTemplate = template;
            this.CurrentTemplate = template;

            cboRegion.Items.Clear();
            cboRegion.Items.AddRange(template.Regions.Select(r => r.RegionName).ToArray());

            cboTemplates.SelectedItem = template.TemplateName;
        }

        private void SetCurrentRegion(TemplateRegion region)
        {
            this.templateRegionSelector.CurrentRegion = region;
            this.CurrentRegion = region;
            chkBold.Checked = region.Bold;
            chkInvert.Checked = region.Invert;
            chkItalic.Checked = region.Italic;
            chkCenter.Checked = region.Center;

            cboRegion.SelectedItem = region.RegionName;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            JsonSerializer serializer = new JsonSerializer();

            using (TextWriter writer = File.CreateText(ConfigurationManager.AppSettings["TemplateFilePath"]))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
                {
                    jsonWriter.Formatting = Formatting.Indented;
                    serializer.Serialize(jsonWriter, this.Templates);
                }
            }

            lblSaveResult.Text = "Save complete.";
        }
    }
}
