using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleCardTemplates.Engine.Templates;


namespace SimpleCardTemplates.Controls
{
    public partial class TemplateRegionSelector : UserControl
    {

        public event EventHandler<TemplateRegion> OnRegionChange;

        private Template _template;

        public Template BoundTemplate 
        { 
            get
            {
                return _template;
            }
            set
            {
                if (value != null)
                {
                    pbImage.ImageLocation = value.TemplatePath;
                    _template = value;
                }
            }
        }

        public TemplateRegion CurrentRegion { get; set; }

        public bool _isRegionDrawing { get; set; }

        public TemplateRegionSelector()
        {
            InitializeComponent();

            _isRegionDrawing = false;

            this.pbImage.MouseDown += pbImage_MouseDown;
            this.pbImage.MouseUp += pbImage_MouseUp;
            this.pbImage.MouseMove += pbImage_MouseMove;
            this.pbImage.Paint += pbImage_Paint;
        }


        void pbImage_Paint(object sender, PaintEventArgs e)
        {
            if (this.BoundTemplate != null)
            {
                SolidBrush brush = new SolidBrush(Color.Yellow);
                Pen borderPen = new Pen(Color.Black);
                Pen selectedBorderPen = new Pen(Color.Black, 2.0f);
                Font drawFont = new Font("Arial", 12);
                SolidBrush fontBrush = new SolidBrush(Color.Red);

                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                stringFormat.Trimming = StringTrimming.Character;

                foreach (var region in this.BoundTemplate.Regions)
                {
                    Rectangle actualRectangle = GetActualPosition(region.Region);

                    e.Graphics.FillRectangle(brush, actualRectangle);
                    if (region == this.CurrentRegion)
                    {
                        e.Graphics.DrawRectangle(selectedBorderPen, actualRectangle);
                    }
                    else
                    {
                        e.Graphics.DrawRectangle(borderPen, actualRectangle);
                    }

                    e.Graphics.DrawString(region.RegionName, drawFont, fontBrush, actualRectangle, stringFormat);
                }

                brush.Dispose();
                borderPen.Dispose();
                drawFont.Dispose();
                fontBrush.Dispose();

                e.Graphics.Flush();
            }
        }


        void pbImage_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.BoundTemplate != null)
            {
                Point point = GetImagePosition(e);

                foreach (var region in this.BoundTemplate.Regions)
                {
                    if (region.Region.Contains(point))
                    {
                        this.CurrentRegion = region;
                        OnRegionChange(this, this.CurrentRegion);
                        return;
                    }
                }

                if (this.CurrentRegion != null)
                {
                    _isRegionDrawing = true;
                    this.CurrentRegion.SetOriginPosition(point);
                    this.CurrentRegion.SetDestinationPosition(point);
                }
            }
        }


        void pbImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isRegionDrawing && this.CurrentRegion != null)
            {
                this.CurrentRegion.SetDestinationPosition(GetImagePosition(e));
            }
            pbImage.Refresh();
        }

        void pbImage_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isRegionDrawing)
            {
                _isRegionDrawing = false;

                if (this.CurrentRegion != null)
                {
                    this.CurrentRegion.SetDestinationPosition(GetImagePosition(e));
                }
                pbImage.Refresh();
            }
        }

        private Rectangle GetActualPosition(Rectangle imageRectangle)
        {
            
            int actualXPosition = (int)(((float)this.pbImage.Width / (float)this.pbImage.Image.Width) * (float)imageRectangle.Location.X);
            int actualYPosition = (int)(((float)this.pbImage.Height / (float)this.pbImage.Image.Height) * (float)imageRectangle.Location.Y);
            int actualWidth = (int)(((float)this.pbImage.Width / (float)this.pbImage.Image.Width) * (float)imageRectangle.Size.Width);
            int actualHeight = (int)(((float)this.pbImage.Height / (float)this.pbImage.Image.Height) * (float)imageRectangle.Size.Height);

            return new Rectangle(actualXPosition, actualYPosition, actualWidth, actualHeight);
        }

        private Point GetImagePosition(MouseEventArgs e)
        {
            int actualXPosition = (e.X * this.pbImage.Image.Width) / this.pbImage.Width;
            int actualYPosition = (e.Y * this.pbImage.Image.Height) / this.pbImage.Height;

            Point point = new Point(
                actualXPosition,
                actualYPosition);
            return point;
        }

    }
}
