using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ControllingArduino
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            
        }

        #region some variables
        Bitmap distanceBMP = new Bitmap(1024, 660);
        Point origin = new Point(1024 / 2, 660 - 1);
#endregion
        #region Draw point
        private void DrawPoint(int r, int degrees, int light)
        { 
            Pen redPen = new Pen(Color.Red, 3);
            double x = r * Math.Cos((double)degrees * Math.PI / 180) ;
            double y = r * Math.Sin((double)degrees * Math.PI / 180) ;
            Color lightColor = new Color();
            lightColor = Color.FromArgb((light) % 255, (light ) % 255, (light) % 255);
            Pen lightPen = new Pen(lightColor, 7);
            /* Draw the lines */
            using (var graphics = Graphics.FromImage(distanceBMP))
            {

                graphics.DrawLine(lightPen, (int)origin.X, (int)origin.Y, (int)x + origin.X, (int)-y + origin.Y);

            }
            /*  Draw the points   */
            for(int i=0;i<10;i++)
                for(int j=0;j<10;j++)
            distanceBMP.SetPixel((int)x + origin.X -i, (int)-y + origin.Y-j, Color.Red);
            
            
            panel1.BackgroundImage = null;
            panel1.BackgroundImage = distanceBMP;
        }
#endregion
        #region Graph view button
        private void graphViewButton_Click(object sender, EventArgs e)
        {
            
            
            Bitmap lightBMP = new Bitmap(panel1.Width, panel1.Height);
            distanceBMP.SetPixel(origin.X, origin.Y, Color.Red);
            Pen blackPen = new Pen(Color.Black, 3);

            int x1 = panel1.Width/2;
            int y1 = panel1.Height;
            int x2 = panel1.Width / 2;
            int y2 = 0;
            // Draw line to screen.
            using (var graphics = Graphics.FromImage(distanceBMP))
            {
                graphics.DrawLine(blackPen, x1, y1, x2, y2);
            }

             x1 = 0;
             y1 = panel1.Height;
             x2 = panel1.Width ;
             y2 = panel1.Height;
            // Draw line to screen.
            using (var graphics = Graphics.FromImage(distanceBMP))
            {
                graphics.DrawLine(blackPen, x1, y1, x2, y2);
            }


            panel1.BackgroundImage = distanceBMP;
        }
#endregion
        #region Form load
        private void Form1_Load(object sender, EventArgs e)
        {
            string[] serialPorts = System.IO.Ports.SerialPort.GetPortNames();
            cboPorts.Items.AddRange(serialPorts);
            graphViewButton_Click(this, e);
            
            try
            {
                cboPorts.SelectedIndex = 0;
                cboBaud.SelectedIndex = 2;
            }
            catch (Exception oops)
            { 
                
            }
        }
#endregion
        #region Start button
        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = cboPorts.SelectedItem.ToString();

                serialPort1.BaudRate = Convert.ToInt32(cboBaud.SelectedItem.ToString());

                if (!serialPort1.IsOpen)
                {
                    btnStart.Enabled = false;
                    btnStop.Enabled = true;
                    serialPort1.Open();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("" + err);
            }
            finally { 
            
            }
        }
#endregion
        #region Stop button
        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {


                if (serialPort1.IsOpen)
                {
                    btnStart.Enabled = true;
                    btnStop.Enabled = false;
                    serialPort1.Close();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("" + err);
            }
            finally
            {

            }
        }
#endregion
        #region Send button
        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                    timer1.Start();
                
            }
            catch (Exception oops)
            { 
            
            }
        }

        delegate void SetTextCallback(string text);
        private int isDistance = 0;
        private int distance=0, degrees=0,noOfIterations,light=0;
#endregion
        #region Set text
        private void SetText(string text)
        {
            if (this.txtOutput.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.BeginInvoke(d, new object[] { text });
            }
            else
            {
                try
                {
                    String newText = text.Trim();
                    if (newText == "-10")
                        ;
                    else
                    {
                        
                        if (isDistance == 0)
                        {

                            distance = Int16.Parse(newText);
                            isDistance++;
                        }
                        else if (isDistance == 1)
                        {
                            isDistance++;
                            degrees = Int16.Parse(newText);
                        }
                        else if(isDistance == 2)
                        {
                            isDistance ++ ;
                            light = Int16.Parse(newText);
                        }
                    }
                    if(isDistance == 3)
                    {
                        isDistance = 0;
                        double scaledDistance, scaledLight;
                        scaledDistance = distance * 1.0;
                        
                        scaledLight = light * 1.3;
                        DrawPoint((int)scaledDistance, (int)degrees,(int) scaledLight);
                        txtOutput.AppendText(distance + " " + degrees + "  " + light + "\n");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error : " + ex);
                }
            }
        }
#endregion
        #region Serial port - Data received
        private void serialPort1_DataReceived_1(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                SetText(serialPort1.ReadLine());
            }

            catch (Exception ex)
            {
                SetText(ex.ToString());
            }
        }
#endregion
        #region Reset button
        private void btnReset_Click(object sender, EventArgs e)
        {
            distanceBMP = new Bitmap(1024, 660);
            panel1.BackgroundImage = distanceBMP;
            txtOutput.Clear();
            Form1_Load(this, e);

        }
        private int time = 0;
        #endregion
        #region TIMER
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                time++;
                if (time < 170)
                    if (serialPort1.IsOpen)
                        serialPort1.Write(1 + "");
                    else
                        return;
                else
                {
                    timer1.Stop();
                    time = 0;
                }
            }
            catch (Exception ex)
            { 
            
            }
        }
        #endregion
    }
}
