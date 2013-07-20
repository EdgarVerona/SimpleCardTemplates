using SimpleCardTemplates.Engine.Templates;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCardTemplates.Console
{
    class Program
    {
        static void Main(string[] args)
        {
			List<Template> templates = Template.GetListFromFile(ConfigurationManager.AppSettings["TemplateFilePath"]);

			using (SqlConnection conn = new SqlConnection(@"CONNECTION_TO_SOME_DATABASE"))
			{
				conn.Open();

				foreach (Template template in templates)
				{    
					using (SqlCommand command = new SqlCommand(string.Format("SELECT * FROM dbo.{0}", template.TemplateName), conn))
					{
						using (SqlDataReader reader = command.ExecuteReader())
						{

							while (reader.Read())
							{
								Bitmap cardPng = (Bitmap)System.Drawing.Image.FromFile(template.TemplatePath, true);
								Bitmap card = new Bitmap(cardPng.Width, cardPng.Height);
								Graphics grPhoto = Graphics.FromImage(card);
								grPhoto.DrawImage(cardPng, new Rectangle(0, 0, card.Width, card.Height), 0, 0, card.Width, card.Height, GraphicsUnit.Pixel);

								Graphics cardGraphics = Graphics.FromImage(card);

								SolidBrush brush = new SolidBrush(Color.Black);

								int id = 0;

								// Draw all the fields onto the card.
								for (int fieldIndex = 0; fieldIndex < reader.FieldCount; fieldIndex++)
								{
									string fieldName = reader.GetName(fieldIndex);

									if (fieldName == "Id")
									{
										id = reader.GetInt32(fieldIndex);
									}

									var region = template.Regions.FirstOrDefault(r => r.RegionName == fieldName);

									if (region != null)
									{
										string value = reader[fieldIndex].ToString();

										StringFormat format = new StringFormat();
										if (region.Center)
										{
											format.Alignment = StringAlignment.Center;
											format.LineAlignment = StringAlignment.Center;
										}

										Font font = GetFontForStringAndRegion(value, region, cardGraphics);

										if (region.Invert)
										{
											cardGraphics.TranslateTransform(region.Region.X + region.Region.Width, region.Region.Y + region.Region.Height);
											cardGraphics.RotateTransform(180);
											cardGraphics.DrawString(
												value,
												font,
												brush,
												new RectangleF(0, 0, region.Region.Width, region.Region.Height),
												format);
											cardGraphics.ResetTransform();
										}
										else
										{
											cardGraphics.DrawString(
												value,
												font,
												brush,
												region.Region,
												format);
										}

										font.Dispose();
									}
								}

								brush.Dispose();
								cardGraphics.Flush();

                                string outputRootPath = ConfigurationManager.AppSettings["OutputRootPath"];

								if (id > 0)
								{
									if (!Directory.Exists(Path.Combine(outputRootPath, template.TemplateName)))
									{
                                        Directory.CreateDirectory(Path.Combine(outputRootPath, template.TemplateName));
									}
									card.Save(Path.Combine(outputRootPath, template.TemplateName, id.ToString(), ".png"), ImageFormat.Png);
								}
								else
								{
									throw new Exception("What Id");
								}

								card.Dispose();
							}
						}
					}
				}

				conn.Close();
			}
        }


        private static Font GetFontForStringAndRegion(string value, TemplateRegion region, Graphics g)
        {
            int fontPoint = 20;

            while (true)
            {
                FontStyle fontStyle = FontStyle.Regular;

                if (region.Bold)
                {
                    fontStyle = FontStyle.Bold | fontStyle;
                }
                if (region.Italic)
                {
                    fontStyle = FontStyle.Italic | fontStyle;
                }
                Font testFont = new Font("Arial", 14, fontStyle);

                SizeF size = g.MeasureString(value, testFont, region.Region.Width);

                if (size.Height > region.Region.Height)
                {
                    fontPoint -= 2;

                    if (fontPoint == 0)
                    {
                        return testFont;
                    }
                }
                else
                {
                    return testFont;
                }
            }
        }

    }
}
