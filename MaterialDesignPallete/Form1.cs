using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialDesignPallete
{
    public partial class Form1 : Form
    {
        private const string THIRD_BUTTON_TEXT = "Verison 1.0\nby\nFuLLKade.IR";
        private const string COPIED_MESSAGE = "Copied!";
        private const int COPIED_INTERVAL = 300;

        public Form1()
        {
            InitializeComponent();

            this.Width = 700;
            this.Height = 480;

            FormMover.AddMover(this, this);
            FormMover.AddMover(this, pnlTitleBar);
            FormMover.AddMover(this, lblTitle1);
            FormMover.AddMover(this, lblTitle2);

            InitilizeMainColors();
            ClickMainColor(Properties.Settings.Default.LastMainColor);
        }

        /// <summary>
        /// Adds the buttons of main colors to the related panel
        /// </summary>
        private void InitilizeMainColors()
        {
            pnlTable.Controls.Clear();

            var cellSize = pnlTable.Width / 5;
            var cellBorderSize = 2;

            List<MaterialColors> values = Enum.GetValues(typeof(MaterialColors)).Cast<MaterialColors>().ToList();
            var eIndex = 0;

            // Rows
            for (int i = 0; i < 4; i++)
            {
                // Cols
                for (int j = 0; j < 5; j++)
                {
                    var btnMainColor = new Button();
                    pnlTable.Controls.Add(btnMainColor);

                    btnMainColor.UseVisualStyleBackColor = true;
                    btnMainColor.Name = "btnMainColor" + i;
                    btnMainColor.Location = new Point(cellSize * j, cellSize * i);
                    btnMainColor.Size = new Size(cellSize, cellSize);
                    btnMainColor.TabIndex = eIndex;

                    // The buttons Border
                    btnMainColor.FlatStyle = FlatStyle.Flat;
                    btnMainColor.FlatAppearance.BorderColor = btnMainColor.Parent.BackColor;
                    btnMainColor.FlatAppearance.BorderSize = 2;

                    // The third button (row 1, col 3)
                    if (i == 0 && j == 2)
                    {
                        btnMainColor.Font = new Font("Segoe UI", 8F, FontStyle.Bold, GraphicsUnit.Point);
                        btnMainColor.BackColor = btnMainColor.Parent.BackColor;
                        btnMainColor.ForeColor = Color.White;
                        btnMainColor.Text = THIRD_BUTTON_TEXT;
                        btnMainColor.FlatAppearance.MouseOverBackColor = btnMainColor.BackColor;
                        btnMainColor.BackColorChanged += (s, e) => {
                            btnMainColor.FlatAppearance.MouseOverBackColor = btnMainColor.BackColor;
                        };
                        FormMover.AddMover(this, btnMainColor);
                    }
                    else
                    {
                        btnMainColor.BackColor = values[eIndex].MainColor();
                        btnMainColor.ForeColor = DetermineTextColor(btnMainColor.BackColor);
                        btnMainColor.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
                        btnMainColor.Text = values[eIndex].Title();
                        btnMainColor.Tag = values[eIndex];
                        btnMainColor.Click += BtnMainColor_Click;
                        eIndex++;
                    }
                }
            }

            pnlSubColors.Height = cellSize * 4 + (4 * (cellBorderSize - 1));
        }

        /// <summary>
        /// Adds the buttons of sub colors to the related panel
        /// </summary>
        /// <param name="mainColor"></param>
        private void InitilizeSubColors(MaterialColors mainColor)
        {
            // get all sub colors
            var colors = mainColor.Colors();

            // add the buttons of sub colors just once
            if (pnlSubColors.Controls.Count == 0)
            {
                var cellHeight = pnlSubColors.Height / colors.Length;
                var cellWidth = pnlSubColors.Width;
                for (var i = 0; i < colors.Length; i++)
                {
                    var btnSubColor = new Button();
                    pnlSubColors.Controls.Add(btnSubColor);
                    btnSubColor.UseVisualStyleBackColor = true;
                    btnSubColor.Location = new Point(0, cellHeight * i);
                    btnSubColor.Size = new Size(cellWidth, cellHeight);
                    btnSubColor.TabIndex = i;
                    btnSubColor.FlatStyle = FlatStyle.Flat;
                    btnSubColor.FlatAppearance.BorderColor = pnlTable.BackColor;
                    btnSubColor.FlatAppearance.BorderSize = 2;
                    btnSubColor.Click += BtnSubColor_Click;
                }
            }

            for (var i = 0; i < pnlSubColors.Controls.Count; i++)
            {
                var number = 50;
                if (i > 0) number = 100 * i;
                var ctrlSubColor = pnlSubColors.Controls[i];
                ctrlSubColor.Text = number + "     " + colors[i];
                ctrlSubColor.BackColor = ColorTranslator.FromHtml(colors[i]);
                ctrlSubColor.ForeColor = DetermineTextColor(ctrlSubColor.BackColor);
            }
        }

        /// <summary>
        /// Determines the fore (text) color of a button by background color
        /// </summary>
        /// <param name="bgColor">Control background color</param>
        /// <returns>The fore color</returns>
        public Color DetermineTextColor(Color bgColor)
        {
            double luma = ((0.299 * bgColor.R) + (0.587 * bgColor.G) + (0.114 * bgColor.B)) / 255;
            return luma > 0.5 ? Color.Black : Color.White;
        }

        /// <summary>
        /// Randomly clicks on a main color
        /// </summary>
        private void ClickMainColor(int index = -1)
        {
            if (index < 0 || index > pnlTable.Controls.Count)
            {
                Random rnd = new Random();
                index = rnd.Next(0, pnlTable.Controls.Count);
            }
            if (index == 2) index = 3;
            BtnMainColor_Click(pnlTable.Controls[index], null);
        }

        /// <summary>
        /// When the main colors are clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMainColor_Click(object sender, EventArgs e)
        {
            var ctrlMainColor = (Control)sender;
            var materialColor = (MaterialColors)ctrlMainColor.Tag;
            InitilizeSubColors(materialColor);
            Properties.Settings.Default.LastMainColor = ctrlMainColor.TabIndex;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// When the sub colors are clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSubColor_Click(object sender, EventArgs e)
        {
            Button btnSubColor = (Button)sender;
            if (btnSubColor.Text.Equals(COPIED_MESSAGE)) return;
            Clipboard.SetText(ColorTranslator.ToHtml(btnSubColor.BackColor));

            Timer timer = new Timer();
            timer.Interval = COPIED_INTERVAL;
            timer.Tag = btnSubColor.Text; // presaves the sub color text in the timer tag
            btnSubColor.Text = COPIED_MESSAGE;
            timer.Tick += (object sender, EventArgs e) =>
            {
                if (btnSubColor != null && btnSubColor.Text == COPIED_MESSAGE)
                {
                    btnSubColor.Text = timer.Tag.ToString();
                }
                timer.Stop();
                timer = null;
            };
            timer.Start();
        }

        /// <summary>
        /// When the close button is cliked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pbClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// When the minimize button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pbMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}