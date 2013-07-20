using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCardTemplates.Engine.Templates
{

    public class TemplateRegion
    {

        public string RegionName { get; set; }

        [JsonIgnore]
        public Rectangle Region 
        { 
            get
            {
                return new Rectangle(X, Y, Width, Height);
            }
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public bool Invert { get; set; }
        public bool Center { get; set; }

        public TemplateRegion(string regionName, Rectangle region)
        {
            this.RegionName = regionName;
            this.X = region.X;
            this.Y = region.Y;
            this.Width = region.Width;
            this.Height = region.Height;
        }

        public void SetOriginPosition(Point point)
        {
            this.X = point.X;
            this.Y = point.Y;
        }

        public void SetDestinationPosition(Point point)
        {
            Size size = new Size(
                point.X - this.Region.Left,
                point.Y - this.Region.Top
                );

            this.Width = size.Width;
            this.Height = size.Height;
        }

    }

}
