using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using System.Drawing.Drawing2D;

namespace PulseParser
{
    public partial class PulseParser : Form
    {
        public class OneAxisGroup
        {
            public Int32 nLineSeqIdx; //碎线段号
            public Int16 nAxis; //轴号，0=x, 1=y, 2=z
            public Int16 nMotorDir; //方向，0=CW, 1=CCW
            public Int32 nPulseNum; //理想脉冲数
            public float nPulsePrd; //理想脉冲周期，unit in 20ns

            public OneAxisGroup()
            {
                this.nLineSeqIdx = 0; //碎线段号
                this.nAxis = 0; //轴号，0=x, 1=y, 2=z
                this.nMotorDir = 0; //方向，0=CW, 1=CCW
                this.nPulseNum = 0; //理想脉冲数
                this.nPulsePrd = 0; //理想脉冲周期，unit in 20ns
            }

            public OneAxisGroup(Int32 nLineSeqIdx, 
                            Int16 nAxis, //轴号，0=x, 1=y, 2=z
                            Int16 nMotorDir, //方向，0=CW, 1=CCW
                            Int32 nPulseNum, //理想脉冲数
                            Int32 nPulsePrd //理想脉冲周期，unit in 20ns
                )
            {
                this.nLineSeqIdx = nLineSeqIdx; //碎线段号
                this.nAxis = nAxis; //轴号，0=x, 1=y, 2=z
                this.nMotorDir = nMotorDir; //方向，0=CW, 1=CCW
                this.nPulseNum = nPulseNum; //理想脉冲数
                this.nPulsePrd = nPulsePrd; //理想脉冲周期，unit in 20ns
            }
        };

        public class OneSyncGroup
        {
            public Int64 nTick;
            public Int64 nLineSeqIdx; //碎线段号
            public Int64 nSyncSeq;
            public float nInterval;

            public OneAxisGroup x;
            public OneAxisGroup y;
            public OneAxisGroup z;

            public OneSyncGroup()
            {
                nTick = 0;
                nLineSeqIdx = 0;
                nSyncSeq = 0;
                nInterval = 0;

                x = new OneAxisGroup();
                y = new OneAxisGroup();
                z = new OneAxisGroup();
            }
        };

        [StructLayout(LayoutKind.Explicit)]
        struct FPGApulse
        {
            [FieldOffset(0)]
            public Int32 nLineSeqIdx; //碎线段号

            [FieldOffset(4)]
            public Int16 nAxis; //轴号，0=x, 1=y, 2=z

            [FieldOffset(6)]
            public Int16 nMotorDir; //方向，0=CW, 1=CCW

            [FieldOffset(8)]
            public Int32 nPulseNum; //理想脉冲数

            [FieldOffset(12)]
            public Int32 nPulsePrd; //理想脉冲周期，unit in 20ns
        }

        public Boolean formInited = false;
        public static int running = 0;
        public static int stopped = 1;

        public static Int64 timeTotal = 0;
        public static Int64 totalPulses = 0;
        public static Int64 lastFinished = 0;
        public static Int64 finishedPulses = 0;

        public static String _fpgaFile = "";
        public static List<OneSyncGroup> _pulseTicks = new List<OneSyncGroup>();
        public static DataTable _timeTable = new DataTable();

        public static Int64 _viewPortStartTick = 0;
        public static Int64 _viewPortPanTickOffset = 0;
        public static Int64 _viewPortTickSpan = 10 * 20 * 8000;
        public static double _singal = 30;

        public static int _panStep = 10000;
        public static double tickScaleX = 1.0;
        public static double tickScaleY = 1.0;

        public static int[] totalAxisPulses = new int[3];

        public static Boolean _dragging = false;
        public static Cursor _panCursor = Cursors.Hand;
        public static Point _mouseDownLocation = new Point();
        public static Point _mouseLastLocation = new Point();
        public static Point _mouseOffset = new Point();

        public PulseParser()
        {
            InitializeComponent();
        }

        private void PulseParser_Load(object sender, EventArgs e)
        {
            this.textBoxSpan.Text = String.Format("{0}", _viewPortTickSpan / 20); //10 * 20 * 8000;

            Bitmap cur = new Bitmap(Properties.Resources.horz_cursor);
            cur.MakeTransparent(cur.GetPixel(0, 0));
            Graphics g = Graphics.FromImage(cur);
            IntPtr ptr = cur.GetHicon();
            _panCursor = new Cursor(ptr);

            ToolTip toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 2000;
            toolTip1.InitialDelay = 500;
            //toolTip1.ReshowDelay = 500;
            toolTip1.SetToolTip(this.pictureBoxScoper, "使用鼠标滚轮放大、或缩小");

            ToolTip toolTip2 = new ToolTip();
            toolTip2.AutoPopDelay = 2000;
            toolTip2.InitialDelay = 500;
            //toolTip2.ReshowDelay = 500;
            toolTip2.SetToolTip(this.hScrollBarPan, "使用鼠标滚轮向左、右平移");

            _timeTable.Columns.Add("tick", typeof(Int64));
            _timeTable.Columns.Add("span", typeof(Int64));
            _timeTable.Columns.Add("index", typeof(Int64));
            _timeTable.PrimaryKey = new DataColumn[] { _timeTable.Columns["tick"] };

            this.trackBar.SetRange(8, 800); //10 * 20 * 8000

            formInited = false;
            this.timerUpdate.Enabled = true;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Open FPGA Pulse File";
            fdlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            fdlg.Filter = "FPGA files (*.fpga)|*.fpga|All files (*.*)|*.*";
            //fdlg.FilterIndex = 2;
            //fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
                _fpgaFile = fdlg.FileName;

            if (File.Exists(_fpgaFile))
            {
                //display.DoubleBuffering = true;
                //display.Smoothing = SmoothingMode.HighSpeed;
                //display.PlaySpeed = 5;
                //display.Refresh();

                byte[] readBytes = File.ReadAllBytes(_fpgaFile);

                Int64 numPulses = (Int64)readBytes.Length / Marshal.SizeOf(typeof(FPGApulse));
                totalPulses = numPulses;

                Interlocked.Exchange(ref running, 1);
                Interlocked.Exchange(ref stopped, 0);

                lastFinished = 0;
                finishedPulses = 0;

                Interlocked.Exchange(ref totalAxisPulses[0], 0);
                Interlocked.Exchange(ref totalAxisPulses[1], 0);
                Interlocked.Exchange(ref totalAxisPulses[2], 0);

                this.buttonBrowse.Enabled = false;
                this.timerUpdate.Enabled = true;

                Thread thread = new Thread(() => LoadPulse(_fpgaFile, readBytes, numPulses));
                thread.Start();
            }
        }

        private void LoadPulse(String fpgFile, byte[] readBytes, Int64 numPulses)
        {
            _pulseTicks.Clear();
            _timeTable.Rows.Clear();

            var pulseArray = new FPGApulse[numPulses];
            var pinnedHandle = GCHandle.Alloc(pulseArray, GCHandleType.Pinned);
            Marshal.Copy(readBytes, 0, pinnedHandle.AddrOfPinnedObject(), readBytes.Length);
            pinnedHandle.Free();

            OneSyncGroup step = new OneSyncGroup();

            String exportFile = _fpgaFile;
            exportFile = exportFile.Replace(".fpga", ".txt");
            StringBuilder export = new StringBuilder();
            using(TextWriter writer = File.CreateText(exportFile))
            {
                Boolean hasX = false;
                Boolean hasY = false;
                Boolean hasZ = false;
                Int64 maxInterval = 0;
                Int64 timeTicks = 0;

                timeTotal = 0;

                Int64 lastLineSeq = -1;
                Int64 lastSyncNo = 0;
                int lastAxis = 2;
                for (Int64 i = 0; i < numPulses; i++)
                {
                    if (pulseArray[i].nAxis >= lastAxis)
                    { //sync
                        lastAxis = 2;
                        export.Append(String.Format("Sync: {0}\r\n", lastSyncNo));

                        if (hasX || hasY || hasZ)
                        {
                            step.nLineSeqIdx = lastLineSeq;
                            step.nSyncSeq = lastSyncNo ++;
                            step.nInterval = maxInterval;
                            step.nTick = timeTicks;

                            DataRow row = _timeTable.NewRow();
                            row["tick"] = timeTicks;
                            row["span"] = maxInterval;
                            row["index"] = _pulseTicks.Count;
                            _timeTable.Rows.Add(row);

                            _pulseTicks.Add(step);
                            timeTicks += maxInterval;
                            timeTotal += maxInterval;
                        }

                        hasX = false;
                        hasY = false;
                        hasZ = false;
                        maxInterval = 0;
                        step = new OneSyncGroup();
                    }

                    export.Append(String.Format("{0}-{1}-{2}-{3}-{4}\r\n", pulseArray[i].nLineSeqIdx, pulseArray[i].nAxis, pulseArray[i].nMotorDir, pulseArray[i].nPulseNum, pulseArray[i].nPulsePrd));
                    if (export.Length > 8192)
                    {
                        writer.Write(export.ToString());
                        export.Clear();
                    }

                    if (pulseArray[i].nAxis == 2)
                    {
                        hasZ = true;
                        totalAxisPulses[2] += pulseArray[i].nPulseNum;
                        step.z = new OneAxisGroup(pulseArray[i].nLineSeqIdx, pulseArray[i].nAxis, pulseArray[i].nMotorDir, pulseArray[i].nPulseNum, pulseArray[i].nPulsePrd);
                    }
                    else if (pulseArray[i].nAxis == 1)
                    {
                        hasY = true;
                        totalAxisPulses[1] += pulseArray[i].nPulseNum;
                        step.y = new OneAxisGroup(pulseArray[i].nLineSeqIdx, pulseArray[i].nAxis, pulseArray[i].nMotorDir, pulseArray[i].nPulseNum, pulseArray[i].nPulsePrd);
                    }
                    else if (pulseArray[i].nAxis == 0)
                    {
                        hasX = true;
                        totalAxisPulses[0] += pulseArray[i].nPulseNum;
                        step.x = new OneAxisGroup(pulseArray[i].nLineSeqIdx, pulseArray[i].nAxis, pulseArray[i].nMotorDir, pulseArray[i].nPulseNum, pulseArray[i].nPulsePrd);
                    }

                    if (maxInterval < pulseArray[i].nPulsePrd)
                        maxInterval = pulseArray[i].nPulsePrd;

                    lastAxis = pulseArray[i].nAxis;
                    lastLineSeq = pulseArray[i].nLineSeqIdx;

                    finishedPulses ++;
                }
                export.Append(String.Format("Last Sync: {0}\r\n", lastSyncNo));
                if (export.Length > 0)
                {
                    writer.Write(export.ToString());
                    export.Clear();
                }

                if (hasX || hasY || hasZ)
                {
                    step.nLineSeqIdx = lastLineSeq;
                    step.nSyncSeq = lastSyncNo++;
                    step.nInterval = maxInterval;
                    step.nTick = timeTicks;

                    DataRow row = _timeTable.NewRow();
                    row["tick"] = timeTicks;
                    row["span"] = maxInterval;
                    row["index"] = _pulseTicks.Count;
                    _timeTable.Rows.Add(row);

                    _pulseTicks.Add(step);
                    timeTicks += maxInterval;
                    timeTotal += maxInterval;
                }
            }

            Interlocked.Exchange(ref stopped, 1);
        }

        public Boolean RedrawGraph()
        {
            if (FormWindowState.Minimized == this.WindowState)
                return false;

            Bitmap bitmap = new Bitmap(this.pictureBoxScoper.Width, this.pictureBoxScoper.Height);
            //Graphics gr = this.pictureBoxScoper.CreateGraphics();
            using (Graphics gr = Graphics.FromImage(bitmap))
            {
                gr.Clear(Color.White);

                Pen penX = new Pen(Color.Red, 0.015F);
                Pen penY = new Pen(Color.Green, 0.015F);
                Pen penZ = new Pen(Color.Blue, 0.015F);

                //gr.TranslateTransform(halfX, halfY);
                //gr.ScaleTransform(scaleX, scaleY);
                //gr.ResetClip();

                double graphHeight = 100; //包括正负半轴

                int nCurves = 3;
                int gMargin = 10;
                int gWidth = this.pictureBoxScoper.Width - 2 * gMargin;
                int gHeight = this.pictureBoxScoper.Height - 2 * gMargin;

                gr.Clip = new Region(new Rectangle(gMargin, gMargin, gWidth, gHeight));

                tickScaleX = (double)gWidth / _viewPortTickSpan;
                tickScaleY = (double)gHeight / (graphHeight * nCurves);

                int gHorzOff = gMargin;
                int gVertOffX = (int)((double)1 * gHeight / (2 * nCurves)) + gMargin;
                int gVertOffY = (int)((double)3 * gHeight / (2 * nCurves)) + gMargin;
                int gVertOffZ = (int)((double)5 * gHeight / (2 * nCurves)) + gMargin;

                DataRow[] rows = getIndex((_viewPortStartTick + _viewPortPanTickOffset), _viewPortTickSpan);
                if (rows.Length <= 0)
                    return false;
                UInt32 firstIdx = Convert.ToUInt32(rows[0]["index"]);

                Font stringFont = new Font("Arial", 8);
                SizeF stringSize = new SizeF();

                OneSyncGroup sync;
                Pen penGroup = new Pen(Color.Gray);

                gr.DrawLine(penGroup, gMargin, gVertOffX, gWidth + gMargin, gVertOffX);
                gr.DrawLine(penGroup, gMargin, gVertOffY, gWidth + gMargin, gVertOffY);
                gr.DrawLine(penGroup, gMargin, gVertOffZ, gWidth + gMargin, gVertOffZ);

                for (UInt32 idx = firstIdx; idx < firstIdx + rows.Length; idx++)
                {
                    if (idx < _pulseTicks.Count)
                    {
                        sync = _pulseTicks[(int)idx];

                        Int64 x = (Int64)((sync.nTick - (_viewPortStartTick + _viewPortPanTickOffset)) * tickScaleX);
                        Int64 syncWidth = (Int64)(sync.nInterval * tickScaleX);

                        gr.DrawLine(penGroup, x + gMargin, gMargin, x + gMargin, gHeight + gMargin);
                        if (idx == firstIdx + rows.Length - 1)
                            gr.DrawLine(penGroup, x + syncWidth + gMargin, gMargin, x + syncWidth + gMargin, gHeight + gMargin);

                        DrawPulseGroup(gr, penX, gMargin, gVertOffX, sync.x, sync.nTick, tickScaleX, tickScaleY);
                        DrawPulseGroup(gr, penY, gMargin, gVertOffY, sync.y, sync.nTick, tickScaleX, tickScaleY);
                        DrawPulseGroup(gr, penZ, gMargin, gVertOffZ, sync.z, sync.nTick, tickScaleX, tickScaleY);

                        String syncText = String.Format("{0} / {1}", sync.nSyncSeq, sync.nLineSeqIdx);
                        stringSize = gr.MeasureString(syncText, stringFont);
                        float tx = ((float)syncWidth - stringSize.Width) / 2;
                            if (tx < 0) tx = 0;
                        float ty = ((float)gHeight - stringSize.Height/2);
                        gr.DrawString(syncText, stringFont, Brushes.Black, new PointF(tx + x, ty));

                        int numOffY = 10;
                        if (sync.x.nPulseNum > 0)
                        {
                            String numText = String.Format("{0} x {1}us", sync.x.nPulseNum, (int)(20 * sync.x.nPulsePrd / (sync.x.nPulseNum * 1000)));

                            stringSize = gr.MeasureString(numText, stringFont);
                            float numX = ((float)syncWidth - stringSize.Width) / 2;
                            if (numX < 0) numX = 0;
                            float numY = 0;
                            gr.DrawString(numText, stringFont, Brushes.Black, new PointF(numX + x, numY + numOffY));
                        }

                        if (sync.y.nPulseNum > 0)
                        {
                            //String numText = String.Format("{0}", sync.y.nPulseNum);
                            String numText = String.Format("{0} x {1}us", sync.y.nPulseNum, (int)(20 * sync.y.nPulsePrd / (sync.y.nPulseNum * 1000)));

                            stringSize = gr.MeasureString(numText, stringFont);
                            float numX = ((float)syncWidth - stringSize.Width) / 2;
                            if (numX < 0) numX = 0;
                            float numY = ((float)gHeight / nCurves);
                            gr.DrawString(numText, stringFont, Brushes.Black, new PointF(numX + x, numY + numOffY));
                        }

                        if (sync.z.nPulseNum > 0)
                        {
                            //String numText = String.Format("{0}", sync.z.nPulseNum);
                            String numText = String.Format("{0} x {1}us", sync.z.nPulseNum, (int)(20 * sync.z.nPulsePrd / (sync.z.nPulseNum * 1000)));

                            stringSize = gr.MeasureString(numText, stringFont);
                            float numX = ((float)syncWidth - stringSize.Width) / 2;
                            if (numX < 0) numX = 0;
                            float numY = ((float)2*gHeight / nCurves);
                            gr.DrawString(numText, stringFont, Brushes.Black, new PointF(numX + x, numY + numOffY));
                        }
                    }
                }
            }
            if (this.pictureBoxScoper.Image != null)
                this.pictureBoxScoper.Image.Dispose();
            this.pictureBoxScoper.Image = bitmap;
            return true;
        }

        public void DrawPulseGroup(Graphics g, Pen pen, int horzOffset, int vertOffset, OneAxisGroup axis, Int64 groupStartX, double scaleX, double scaleY)
        {
            double xValue = groupStartX;
            double yValue = (axis.nMotorDir == 0) ? (+_singal) : (-_singal);

            int pulseNum = axis.nPulseNum;
            double pulseWidth = ((double)axis.nPulsePrd / (2 * pulseNum));

            for (int i = 0; i < pulseNum; i++)
            {
                int x1 = (int)((xValue - (_viewPortStartTick + _viewPortPanTickOffset)) * scaleX);
                int x2 = (int)((xValue - (_viewPortStartTick + _viewPortPanTickOffset) + pulseWidth) * scaleX);
                int x3 = (int)((xValue - (_viewPortStartTick + _viewPortPanTickOffset) + 2 * pulseWidth) * scaleX);

                int y = (int)(yValue * scaleY);

                g.DrawLine(pen, x1 + horzOffset, 0 + vertOffset, x1 + horzOffset, y + vertOffset);
                g.DrawLine(pen, x1 + horzOffset, y + vertOffset, x2 + horzOffset, y + vertOffset);
                g.DrawLine(pen, x2 + horzOffset, y + vertOffset, x2 + horzOffset, 0 + vertOffset);
                g.DrawLine(pen, x2 + horzOffset, 0 + vertOffset, x3 + horzOffset, 0 + vertOffset);

                xValue += (float)2 * pulseWidth;
            }
        }

        public DataRow[] getIndex(Int64 tickStart, Int64 tickSpan)
        {
            DataRow[] foundRows = _timeTable.Select(String.Format("tick>={0} AND tick<={1}", tickStart - 2 * tickSpan, tickStart + 2 * tickSpan), "tick ASC");
            return foundRows;
        }

        private void UpdateProgress()
        {
            lock (this)
            {
                double dblPercent = 0.0;
                long percent = 0;
                if (totalPulses > 0)
                {
                    dblPercent = (double)finishedPulses / totalPulses;
                    percent = (100 * finishedPulses / totalPulses);
                }

                Debug.WriteLine(String.Format("Percent: {0}%", percent));

                using (Graphics g = this.progressBar.CreateGraphics())
                {
                    long progress = (long)(dblPercent * this.progressBar.Width);

                    g.Clear(Color.WhiteSmoke);//.LightGray);//.LightSkyBlue);
                    ControlPaint.DrawBorder(g, this.progressBar.ClientRectangle, Color.Lavender, ButtonBorderStyle.Solid);

                    Rectangle rectFinished = new Rectangle(0, 0, (int)progress, this.progressBar.Height);
                    if ((rectFinished.Width > 0) && (rectFinished.Height > 0))
                    {
                        LinearGradientBrush linearGradientBrush;
                        if (false)
                        {
                            linearGradientBrush = new LinearGradientBrush(rectFinished, Color.Red, Color.Yellow, 45);

                            ColorBlend cblend = new ColorBlend(3);
                            cblend.Colors = new Color[3] { Color.Red, Color.Yellow, Color.Green };
                            cblend.Positions = new float[3] { 0f, 0.5f, 1f };

                            linearGradientBrush.InterpolationColors = cblend;
                        }
                        else
                        {
                            linearGradientBrush = new LinearGradientBrush(this.ClientRectangle, Color.Black, Color.Black, 0, false);
                            ColorBlend cb = new ColorBlend();
                            cb.Positions = new[] { 0, 1 / 6f, 2 / 6f, 3 / 6f, 4 / 6f, 5 / 6f, 1 };
                            cb.Colors = new[] { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet };
                            linearGradientBrush.InterpolationColors = cb;
                            linearGradientBrush.RotateTransform(45);
                        }

                        //panel4.BackgroundImage = Gradient2D(panel4.ClientRectangle, Color.Black, Color.FromArgb(255, 0, 255, 0), Color.Red, Color.Yellow);
                        g.FillRectangle(linearGradientBrush, rectFinished);
                        //g.FillRectangle(Brushes.CornflowerBlue, new Rectangle(0, 0, (int)progress, this.progressBar.Height));

                        g.DrawString(percent.ToString() + "%", SystemFonts.DefaultFont, Brushes.Black,
                            new PointF(this.progressBar.Width / 2 - (g.MeasureString(percent.ToString() + "%", SystemFonts.DefaultFont).Width / 2.0F),
                                       this.progressBar.Height / 2 - (g.MeasureString(percent.ToString() + "%", SystemFonts.DefaultFont).Height / 2.0F)));
                    }
                }
            }
        }

        private Bitmap Gradient2D(Rectangle r, Color c1, Color c2, Color c3, Color c4)
        { //panel4.BackgroundImage = Gradient2D(panel4.ClientRectangle, Color.Black, Color.FromArgb(255, 0, 255, 0), Color.Red, Color.Yellow);
            Bitmap bmp = new Bitmap(r.Width, r.Height);

            float delta12R = 1f * (c2.R - c1.R) / r.Height;
            float delta12G = 1f * (c2.G - c1.G) / r.Height;
            float delta12B = 1f * (c2.B - c1.B) / r.Height;
            float delta34R = 1f * (c4.R - c3.R) / r.Height;
            float delta34G = 1f * (c4.G - c3.G) / r.Height;
            float delta34B = 1f * (c4.B - c3.B) / r.Height;
            using (Graphics G = Graphics.FromImage(bmp))
                for (int y = 0; y < r.Height; y++)
                {
                    Color c12 = Color.FromArgb(255, c1.R + (int)(y * delta12R),
                          c1.G + (int)(y * delta12G), c1.B + (int)(y * delta12B));
                    Color c34 = Color.FromArgb(255, c3.R + (int)(y * delta34R),
                          c3.G + (int)(y * delta34G), c3.B + (int)(y * delta34B));
                    using (LinearGradientBrush lgBrush = new LinearGradientBrush(
                          new Rectangle(0, y, r.Width, 1), c12, c34, 0f))
                    { G.FillRectangle(lgBrush, 0, y, r.Width, 1); }
                }
            return bmp;
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            if (!formInited)
            {
                formInited = true;
                this.timerUpdate.Interval = 200;

                using (Graphics g = this.progressBar.CreateGraphics())
                {
                    g.Clear(Color.WhiteSmoke);//.LightGray);//.LightSkyBlue);
                    ControlPaint.DrawBorder(g, this.progressBar.ClientRectangle, Color.Lavender, ButtonBorderStyle.Solid);
                }
            }

            if (running == 1)
            {
                if (stopped > 0)
                {
                    hScrollBarPan.Minimum = 0;
                    hScrollBarPan.Maximum = 1 + (int)(timeTotal / _panStep);

                    Interlocked.Exchange(ref running, 0);

                    this.timerUpdate.Enabled = false;
                    this.buttonBrowse.Enabled = true;

                    RedrawGraph();
                }

                Int64 curCount = finishedPulses;
                if (lastFinished != curCount)
                {
                    lastFinished = curCount;

                    long lastPercent = 0;
                    if (totalPulses > 0)
                        lastPercent = (100 * lastFinished / totalPulses);

                    long percent = 0;
                    if (totalPulses > 0)
                        percent = (100 * lastFinished / totalPulses);

                    UpdateProgress();
                }
            }
        }

        private void PulseParser_SizeChanged(object sender, EventArgs e)
        {
            if (!formInited)
                return;

            using (Graphics g = this.progressBar.CreateGraphics())
            {
                g.Clear(Color.WhiteSmoke);//.LightGray);//.LightSkyBlue);
                ControlPaint.DrawBorder(g, this.progressBar.ClientRectangle, Color.Lavender, ButtonBorderStyle.Solid);
            }

            UpdateProgress();

            RedrawGraph();
        }
        
        private void pictureBoxScoper_MouseWheel(object sender, MouseEventArgs e)
        {
            _viewPortTickSpan = 20 * Convert.ToInt32(this.textBoxSpan.Text);
            if (e.Delta < 0)
                _viewPortTickSpan += 10000;
            else
                _viewPortTickSpan -= 10000;
            if (_viewPortTickSpan < 0)
                _viewPortTickSpan = 10 * 20 * 5000;

            this.textBoxSpan.Text = String.Format("{0}", _viewPortTickSpan/20);

            Debug.WriteLine(String.Format("Wheel: {0}", e.Delta));
        }
        
        private void pictureBoxScoper_Paint(object sender, PaintEventArgs e)
        {
        }
        
        private void hScrollBarPan_Scroll(object sender, ScrollEventArgs e)
        {
            if (!formInited)
                return;

            _viewPortStartTick = this.hScrollBarPan.Value * _panStep;
            RedrawGraph();
        }
        
        private void textBoxSpan_TextChanged(object sender, EventArgs e)
        {
            if (!formInited)
                return;

            _viewPortTickSpan = 20 * Convert.ToInt32(this.textBoxSpan.Text);
            if (_viewPortTickSpan <= 0)
                _viewPortTickSpan = 10 * 20 * 5000;

            RedrawGraph();
        }

        private void pictureBoxScoper_MouseEnter(object sender, EventArgs e)
        {
            if (_pulseTicks.Count <= 0)
                return;

            if (!this.pictureBoxScoper.Focused)
                this.pictureBoxScoper.Focus();
        }

        private void pictureBoxScoper_MouseLeave(object sender, EventArgs e)
        {
            if (_pulseTicks.Count <= 0)
                return;

            if (this.pictureBoxScoper.Focused)
                this.hScrollBarPan.Focus();
        }

        private void pictureBoxScoper_MouseDown(object sender, MouseEventArgs e)
        {
            if (_pulseTicks.Count <= 0)
                return;

            string eventString = "";
            switch (e.Button)
            {
                case MouseButtons.Left:
                    eventString = "L";
                    _dragging = true;
                    _mouseDownLocation = new Point(e.X, e.Y);
                    _mouseLastLocation = new Point(e.X, e.Y);

                    Cursor.Current = _panCursor;
                    break;
                case MouseButtons.Right:
                    eventString = "R";
                    break;
                case MouseButtons.Middle:
                    eventString = "M";
                    break;
                case MouseButtons.XButton1:
                    eventString = "X1";
                    break;
                case MouseButtons.XButton2:
                    eventString = "X2";
                    break;
                case MouseButtons.None:
                default:
                    break;
            }
        }

        private void pictureBoxScoper_MouseMove(object sender, MouseEventArgs e)
        {
            if (_pulseTicks.Count <= 0)
                return;

            int mouseX = e.X;
            int mouseY = e.Y;

            if (_dragging)
            {
                _mouseOffset.X = e.X - _mouseDownLocation.X;
                _mouseOffset.Y = e.Y - _mouseDownLocation.Y;

                if (Math.Abs(e.X - _mouseLastLocation.X) > 20)
                {
                    _mouseLastLocation = new Point(e.X, e.Y);

                    int delta = (int)((double)(-1) * _mouseOffset.X / tickScaleX);
                    if (_viewPortStartTick + delta > Math.Max(timeTotal / 2, timeTotal - _viewPortTickSpan / 2))
                        _viewPortPanTickOffset = Math.Max(timeTotal / 2, timeTotal - _viewPortTickSpan / 2) - _viewPortStartTick;
                    else
                        _viewPortPanTickOffset = delta;

                    if (_viewPortStartTick + _viewPortPanTickOffset <= 0)
                        _viewPortPanTickOffset = (-1) * _viewPortStartTick;

                    RedrawGraph();
                }
            }
        }

        private void pictureBoxScoper_MouseUp(object sender, MouseEventArgs e)
        {
            if (_pulseTicks.Count <= 0)
                return;

            Point mouseUpLocation = new System.Drawing.Point(e.X, e.Y);
            // Show the number of clicks in the path graphic.
            int numberOfClicks = e.Clicks;

            if (_dragging)
            {
                _dragging = false;

                if (_viewPortStartTick + _viewPortPanTickOffset > Math.Max(timeTotal/2, timeTotal-_viewPortTickSpan/2))
                    _viewPortStartTick = Math.Max(timeTotal / 2, timeTotal - _viewPortTickSpan / 2);
                else
                    _viewPortStartTick += _viewPortPanTickOffset;

                if (_viewPortStartTick <= 0)
                    _viewPortStartTick = 0;

                _viewPortPanTickOffset = 0;

                RedrawGraph();

                Cursor.Current = Cursors.Arrow;
            }
        }

        private void trackBar_Scroll(object sender, EventArgs e)
        {
            Debug.WriteLine(e.ToString());
        }

        private void trackBar_ValueChanged(object sender, EventArgs e)
        {
            _viewPortTickSpan = this.trackBar.Value * 10 * 20 * 1000;
            this.textBoxSpan.Text = String.Format("{0}", _viewPortTickSpan / 20); //10 * 20 * 8000;

            Debug.WriteLine(this.trackBar.Value);
        }
    }
}
