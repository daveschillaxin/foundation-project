using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FoundationProject.Controls
{

    public partial class ChartControl 
        : UserControl
    {

        ChartControlType _Type = ChartControlType.Text;
        public ChartControlType Type
        {
            get { return _Type; }
            set
            {
                _Type = value;
                this.Invalidate();
            }
        }

        DataTable _DataSource = null;
        public DataTable DataSource
        {
            get
            {
                if (_DataSource != null && _DataSource.Columns.Contains(_DisplayMember) && _DataSource.Columns.Contains(_ValueMember))
                    return _DataSource;
                else
                    return null; 
            }
            set 
            {
                _DataSource = value;
                LoadColorPallet();
                this.Invalidate();
            }
        }

        List<Color> ColorPallet = new List<Color>();

        ChartControlLegendType _LegendType = ChartControlLegendType.Text;
        public ChartControlLegendType LegendType
        {
            get { return _LegendType; }
            set { _LegendType = value; }
        }

        string _ValueMember = "";
        public string ValueMember
        {
            get { return _ValueMember; }
            set { _ValueMember = value; }
        }

        string _DisplayMember = "";
        public string DisplayMember
        {
            get { return _DisplayMember; }
            set { _DisplayMember = value; }
        }

        public ChartControl()
        {
            InitializeComponent();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {

            base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_DataSource == null)
            {
                e.Graphics.DrawString("No data source", this.Font, new SolidBrush(Color.Black), new Point(7, 7));
                // We cannot do anything with a null data source. Return now.
                return;
            }
            int textY = 7;
            int textHeight = 0;
            int i = 0;
            string text = "";
            // Determine the maximum value.
            double maxValue = 0D;
            double minValue = 0D;
            double total = 0D;
            double currentValue = 0;
            foreach (DataRow row in _DataSource.Rows)
            {
                currentValue = Convert.ToDouble(row[_ValueMember]);
                total += currentValue;
                if (currentValue >= maxValue)
                    maxValue = Convert.ToDouble(row[_ValueMember]);
                if (currentValue <= minValue)
                    minValue = Convert.ToDouble(row[_ValueMember]);
            }
            switch (Type)
            {
                case ChartControlType.Text:
                    #region Text
                    int textWidth = 0;
                    int textX = 7;
                    textHeight = (int)Math.Ceiling(e.Graphics.MeasureString(_DataSource.TableName, this.Font).Height);
                    e.Graphics.DrawString(_DataSource.TableName, this.Font, new SolidBrush(Color.Black), new Point(textX, textY));
                    textY += textHeight;
                    //foreach (DataColumn dataColumn in _DataSource.Columns)
                    //{
                    //    textWidth = 50;
                    //    e.Graphics.DrawString(dataColumn.ColumnName, this.Font, new SolidBrush(Color.Black), new Rectangle(textX, textY, textWidth, textHeight));
                    //    textX += textWidth;
                    //} 
                    textY += textHeight;
                    foreach (DataRow dataRow in _DataSource.Rows)
                    {
                        textX = 7;
                        foreach (DataColumn dataColumn in _DataSource.Columns)
                        {
                            textWidth = 50;
                            e.Graphics.DrawString(dataRow[dataColumn].ToString(), this.Font, new SolidBrush(Color.Black), new Rectangle(textX, textY, textWidth, textHeight));
                            textX += textWidth;
                        }
                        textY += textHeight;
                    }
                    #endregion
                    break;

                case ChartControlType.Bars:
                    #region Bars
                    try
                    {
                        int barCount = _DataSource.Rows.Count;
                        // Draw the legend.
                        if (barCount != 0)
                            textHeight = (int)Math.Ceiling(e.Graphics.MeasureString(_DataSource.Rows[0].ToString(), this.Font).Height);
                        else
                            textHeight = 0;
                        Rectangle legendRegion = new Rectangle(this.Width - 150 + 5, 10, 150 - 10, textHeight * barCount);
                        textY = legendRegion.Y;
                        i = 0;
                        foreach (DataRow row in _DataSource.Rows)
                        {
                            switch (_LegendType)
                            {
                                case ChartControlLegendType.Text:
                                    text = row[_DisplayMember].ToString();
                                    break;

                                case ChartControlLegendType.Value:
                                    text = row[_ValueMember].ToString();
                                    break;

                                case ChartControlLegendType.TextValue:
                                    double value = Convert.ToDouble(row[_ValueMember]);
                                    text = String.Format("{0} ({1})", row[_DisplayMember].ToString(), value.ToString());
                                    break;

                            }
                            e.Graphics.DrawString(text, this.Font, new SolidBrush(Color.Black), new Point(legendRegion.X + textHeight, textY));
                            e.Graphics.FillRectangle(new SolidBrush(ColorPallet[i]), new Rectangle(legendRegion.X, textY, textHeight, textHeight));
                            textY += textHeight;
                            i++;
                        }
                        // Draw the chart.
                        Rectangle chartRegion = new Rectangle(30, 10, this.Width - 40 - 150, this.Height - 40);
                        Pen markerPen = new Pen(Color.LightGray, 2) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot };
                        e.Graphics.DrawLine(markerPen, new Point(chartRegion.X, chartRegion.Y + 2), new Point(chartRegion.X + chartRegion.Width, chartRegion.Y + 2));
                        e.Graphics.DrawLine(markerPen, new Point(chartRegion.X, chartRegion.Y + chartRegion.Height / 2), new Point(chartRegion.X + chartRegion.Width, chartRegion.Y + chartRegion.Height / 2));
                        e.Graphics.DrawLine(markerPen, new Point(chartRegion.X, chartRegion.Y + chartRegion.Height - 2), new Point(chartRegion.X + chartRegion.Width, chartRegion.Y + chartRegion.Height - 2));
                        // Draw the bars now.
                        double barWidth = (double)chartRegion.Width / (double)barCount;
                        double barX = chartRegion.X;
                        i = 0;
                        foreach (DataRow row in _DataSource.Rows)
                        {
                            Color color = ColorPallet[i];
                            double value = Convert.ToDouble(row[_ValueMember]);
                            double barY = 0;
                            double barHeight = 0;
                            if (value == 0)
                                barHeight = 0;
                            else
                            {
                                double multiplier = (double)value / (double)maxValue;
                                barHeight = multiplier * chartRegion.Height;
                            }
                            barY = chartRegion.Height - barHeight + 10;
                            RectangleF barRegion = new RectangleF((float)barX + 2, (float)barY + 2, (float)barWidth - 3, (float)barHeight - 3);
                            e.Graphics.FillRectangle(new SolidBrush(color), barRegion);
                            barX += barWidth;
                            i++;
                        }
                        // Draw text along the left side and bottom.
                        textHeight = (int)Math.Ceiling(e.Graphics.MeasureString(maxValue.ToString(), this.Font).Height);
                        e.Graphics.DrawString(maxValue.ToString(), this.Font, new SolidBrush(Color.Black), new Point(chartRegion.X - 30, chartRegion.Y - (textHeight / 2) + 2));
                        e.Graphics.DrawString(minValue.ToString(), this.Font, new SolidBrush(Color.Black), new Point(chartRegion.X - 30, chartRegion.Y + chartRegion.Height - (textHeight / 2) - 2));
                        double halfValue = (maxValue + minValue) / 2;
                        e.Graphics.DrawString(halfValue.ToString(), this.Font, new SolidBrush(Color.Black), new Point(chartRegion.X - 30, chartRegion.Y + (chartRegion.Height / 2) - (textHeight / 2)));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    #endregion
                    break;

                case ChartControlType.Pie:
                    #region Pie
                    try
                    {
                        int barCount = _DataSource.Rows.Count;
                        // Draw the legend.
                        if (barCount != 0)
                            textHeight = (int)Math.Ceiling(e.Graphics.MeasureString(_DataSource.Rows[0].ToString(), this.Font).Height);
                        else
                            textHeight = 0;
                        Rectangle legendRegion = new Rectangle(this.Width - 150 + 5, 10, 150 - 10, textHeight * barCount);
                        textY = legendRegion.Y;
                        i = 0;
                        foreach (DataRow row in _DataSource.Rows)
                        {
                            switch (_LegendType)
                            {
                                case ChartControlLegendType.Text:
                                    text = row[_DisplayMember].ToString();
                                    break;

                                case ChartControlLegendType.Value:
                                    text = row[_ValueMember].ToString();
                                    break;

                                case ChartControlLegendType.TextValue:
                                    double percentage = 0;
                                    double value = Convert.ToDouble(row[_ValueMember]);
                                    percentage = value / total;
                                    text = String.Format("{0} ({1})", row[_DisplayMember].ToString(), text = percentage.ToString("P"));
                                    break;

                            }
                            e.Graphics.DrawString(text, this.Font, new SolidBrush(Color.Black), new Point(legendRegion.X + textHeight, textY));
                            e.Graphics.FillRectangle(new SolidBrush(ColorPallet[i]), new Rectangle(legendRegion.X, textY, textHeight, textHeight));
                            textY += textHeight;
                            i++;
                        }
                        // Draw the pie.
                        Rectangle chartRegion = new Rectangle(30, 10, this.Width - 40 - 150, this.Height - 40);
                        // Draw the pie chart now.
                        i = 0;
                        double start = 0.0D;
                        double end = 0.0D;
                        double current = 0.0D;
                        foreach (DataRow row in _DataSource.Rows)
                        {
                            Color color = ColorPallet[i];
                            double value = Convert.ToDouble(row[_ValueMember]);
                            current += value;
                            start = end;
                            end = (double)current / total * 360.0D;
                            e.Graphics.FillPie(new SolidBrush(color), chartRegion, (float)start, (float)end - (float)start);
                            i++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    #endregion
                    break;
            }
            //base.OnPaint(e);
        }

        protected override void OnResize(EventArgs e)
        {
            this.Invalidate();
            //base.OnResize(e);
        }

        void LoadColorPallet()
        {
            ColorPallet = new List<Color>();
            Random random = new Random();
            if (_DataSource != null)
                foreach (DataRow row in _DataSource.Rows)
                    ColorPallet.Add(Color.FromArgb(255, (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255)));
        }

    }

}
