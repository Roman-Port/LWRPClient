using LWRPClient.Console.Controls;
using LWRPClient.Console.Panels;
using LWRPClient.Console.Properties;
using LWRPClient.Entities;
using LWRPClient.Layer1;
using LWRPClient.Layer1.Transports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LWRPClient.Console
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private TcpLWRPTransport transport;
        private LWRPConnection conn;

        private const string LAST_IP_FILENAME = "last_address.txt";

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (conn == null) // Connect mode
            {
                //Write last IP address to disk
                File.WriteAllText(LAST_IP_FILENAME, ipBox.Text);

                //Create transport
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipBox.Text), 93);
                transport = new TcpLWRPTransport(ep);
                transport.OnConnected += Conn_OnConnected;
                transport.OnDisconnected += Conn_OnDisconnected;
                transport.OnMessageReceived += Conn_OnMessageReceived;

                //Create connection
                conn = new LWRPConnection(transport, LWRPEnabledFeature.SOURCES | LWRPEnabledFeature.DESTINATIONS | LWRPEnabledFeature.GPI | LWRPEnabledFeature.GPO);
                conn.OnInfoDataReceived += Conn_OnInfoDataReceived;
                conn.Sources.OnBatchUpdate += Conn_OnSrcBatchUpdate;
                BindTabReady(conn.Sources.WaitForReadyAsync(), sourcesTab);
                conn.Destinations.OnBatchUpdate += Conn_OnDstBatchUpdate;
                BindTabReady(conn.Destinations.WaitForReadyAsync(), destinationsTab);
                conn.GPIs.OnBatchUpdate += GPIs_OnBatchUpdate;
                BindTabReady(conn.GPIs.WaitForReadyAsync(), gpiTab);
                conn.GPOs.OnBatchUpdate += GPOs_OnBatchUpdate;
                BindTabReady(conn.GPOs.WaitForReadyAsync(), gpoTab);
                conn.OnConnectionStateUpdate += Conn_OnConnectionStateUpdate;
                conn.OnMeterLevelsUpdated += Conn_OnMeterLevelsUpdated;
                
                conn.Initialize();

                //Set button text
                btnConnect.Text = "Disconnect";
            } else // Disconnect mode
            {
                //Set button text and state
                btnConnect.Enabled = false;
                btnConnect.Text = "Connect";
                btnMeterRefresh.Enabled = true;

                //Dispose of client
                conn.Dispose();
                conn = null;
            }
        }

        private void Conn_OnInfoDataReceived(LWRPConnection conn)
        {
            Invoke((MethodInvoker)delegate
            {
                //Set details
                infoDeviceModel.Text = conn.DeviceModel;
                infoDeviceName.Text = conn.DeviceName;
                infoDeviceVer.Text = conn.DeviceVersion;
                infoLwrpVer.Text = conn.LwrpVersion;
                infoSrcNum.Text = conn.SrcNum.ToString();
                infoDstNum.Text = conn.DstNum.ToString();
                infoGpiNum.Text = conn.GpiNum.ToString();
                infoGpoNum.Text = conn.GpoNum.ToString();

                //Enable connected
                btnApply.Enabled = true;
                btnMeterRefresh.Enabled = true;
            });
        }

        private void Conn_OnSrcBatchUpdate(LWRPConnection conn, ILWRPSource[] sources)
        {
            Invoke((MethodInvoker)delegate
            {
                foreach (var s in sources)
                {
                    //Find the source control belonging to this, if it exists
                    LwSourceControl ctrl = null;
                    foreach (var c in sourcesPanel.Controls)
                    {
                        if (c is LwSourceControl cs && cs.Source == s)
                        {
                            ctrl = cs;
                        }
                    }

                    //If not found, create it
                    if (ctrl == null)
                    {
                        //Create
                        ctrl = new LwSourceControl(s);
                        ctrl.Dock = DockStyle.Top;

                        //Insert
                        sourcesPanel.Controls.Add(ctrl);

                        //Sort
                        sourcesPanel.Controls.SetChildIndex(ctrl, 0);
                    }

                    //Update
                    ctrl.SourceUpdated();
                }
            });
        }

        private void Conn_OnDstBatchUpdate(LWRPConnection conn, ILWRPDestination[] destinations)
        {
            Invoke((MethodInvoker)delegate
            {
                foreach (var s in destinations)
                {
                    //Find the source control belonging to this, if it exists
                    LwDstControl ctrl = null;
                    foreach (var c in dstPanel.Controls)
                    {
                        if (c is LwDstControl cs && cs.Destination == s)
                        {
                            ctrl = cs;
                        }
                    }

                    //If not found, create it
                    if (ctrl == null)
                    {
                        //Create
                        ctrl = new LwDstControl(s);
                        ctrl.Dock = DockStyle.Top;

                        //Insert
                        dstPanel.Controls.Add(ctrl);

                        //Sort
                        dstPanel.Controls.SetChildIndex(ctrl, 0);
                    }

                    //Update
                    ctrl.SourceUpdated();
                }
            });
        }

        private void GPIs_OnBatchUpdate(LWRPConnection conn, ILWRPGpiPort[] updates)
        {
            Invoke((MethodInvoker)delegate
            {
                //Suspend
                gpiTable.SuspendLayout();

                //Loop
                foreach (var s in updates)
                {
                    //Find or create row
                    int rowIndex = s.Index - 1;
                    Control[] row;
                    if (!gpiTable.TryGetRow(rowIndex, out row))
                    {
                        //Create
                        row = new Control[]
                        {
                            new Label
                            {
                                Dock = DockStyle.Fill,
                                TextAlign = ContentAlignment.MiddleCenter,
                                Text = s.Index.ToString()
                            },
                            new GpioPinsControl
                            {
                                Dock = DockStyle.Fill,
                                ReadOnly = true
                            }
                        };

                        //Add
                        gpiTable.InsertRow(rowIndex, row);
                    }

                    //Update
                    ((GpioPinsControl)row[1]).SetPins(s.Pins.ToArray());
                }

                //Resume
                gpiTable.ResumeLayout();
                gpiTable.PerformLayout();
            });
        }

        private void GPOs_OnBatchUpdate(LWRPConnection conn, ILWRPGpoPort[] updates)
        {
            Invoke((MethodInvoker)delegate
            {
                //Suspend
                gpoTable.SuspendLayout();

                //Loop
                foreach (var s in updates)
                {
                    //Find or create row
                    int rowIndex = s.Index - 1;
                    Control[] row;
                    if (!gpoTable.TryGetRow(rowIndex, out row))
                    {
                        //Create GPIO control
                        GpioPinsControl io = new GpioPinsControl
                        {
                            Dock = DockStyle.Fill,
                            ReadOnly = false,
                            Tag = s.Index
                        };
                        io.PinUpdated += GpoPinUserUpdate;

                        //Create the textboxes for fields
                        TextBox nameField = new TextBox
                        {
                            Dock = DockStyle.Top,
                            Tag = s
                        };
                        TextBox addrField = new TextBox
                        {
                            Dock = DockStyle.Top,
                            Tag = s
                        };

                        //Create the row entry
                        row = new Control[]
                        {
                            new Label
                            {
                                Dock = DockStyle.Fill,
                                TextAlign = ContentAlignment.MiddleCenter,
                                Text = s.Index.ToString()
                            },
                            CreateWrappedEditControl("Name", nameField),
                            CreateWrappedEditControl("Address", addrField),
                            io
                        };

                        //Add
                        gpoTable.InsertRow(rowIndex, row);
                    }

                    //Update
                    ((TextBox)row[1].Tag).TextChanged -= NameField_TextChanged;
                    ((TextBox)row[1].Tag).Text = s.Name;
                    ((TextBox)row[1].Tag).BackColor = SystemColors.Window;
                    ((TextBox)row[1].Tag).TextChanged += NameField_TextChanged;

                    ((TextBox)row[2].Tag).TextChanged -= AddrField_TextChanged;
                    ((TextBox)row[2].Tag).Text = s.SourceAddress;
                    ((TextBox)row[2].Tag).BackColor = SystemColors.Window;
                    ((TextBox)row[2].Tag).TextChanged += AddrField_TextChanged;

                    ((GpioPinsControl)row[3]).SetPins(s.Pins.ToArray());
                }

                //Resume
                gpoTable.ResumeLayout();
                gpoTable.PerformLayout();
            });
        }

        private void NameField_TextChanged(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;
            box.BackColor = Color.Yellow;
            ((ILWRPGpoPort)box.Tag).Name = box.Text;
        }

        private void AddrField_TextChanged(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;
            box.BackColor = Color.Yellow;
            ((ILWRPGpoPort)box.Tag).SourceAddress = box.Text;
        }

        private TableLayoutPanel CreateWrappedEditControl(string label, Control content)
        {
            //Create the root layout panel
            TableLayoutPanel root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                Tag = content,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            root.SuspendLayout();

            //Make label control
            Label labelCtrl = new Label
            {
                Text = label,
                Dock = DockStyle.Bottom,
                AutoSize = true
            };

            //Set the middle two to be auto size and the outer to be filling
            root.RowStyles.Clear();
            root.RowStyles.Add(new RowStyle
            {
                Height = 0.5f,
                SizeType = SizeType.Percent
            });
            root.RowStyles.Add(new RowStyle
            {
                SizeType = SizeType.AutoSize
            });
            root.RowStyles.Add(new RowStyle
            {
                SizeType = SizeType.AutoSize
            });
            root.RowStyles.Add(new RowStyle
            {
                Height = 0.5f,
                SizeType = SizeType.Percent
            });

            //Configure the width to fill
            root.ColumnStyles.Clear();
            root.ColumnStyles.Add(new ColumnStyle
            {
                Width = 100,
                SizeType = SizeType.Percent
            });

            //Add all controls
            root.Controls.Add(new Panel(), 0, 0);
            root.Controls.Add(labelCtrl, 0, 1);
            root.Controls.Add(content, 0, 2);
            root.Controls.Add(new Panel(), 0, 3);

            //Resume layout
            root.ResumeLayout();
            root.PerformLayout();

            return root;
        }

        private void GpoPinUserUpdate(GpioPinsControl control, int index, LWRPPinState state)
        {
            //Dispatch
            conn.GPOs[(int)control.Tag - 1].Pins[index] = state;
        }

        private void Conn_OnMessageReceived(ILWRPTransport transport, LWRPMessage message)
        {
            System.Console.WriteLine($"OnMessageReceived: {message}");
        }

        private void Conn_OnDisconnected(ILWRPTransport transport, Exception ex)
        {
            System.Console.WriteLine($"OnDisconnected");
        }

        private void Conn_OnConnected(ILWRPTransport transport)
        {
            System.Console.WriteLine($"OnConnected");
        }

        private void Conn_OnConnectionStateUpdate(LWRPConnection conn, LWRPState state)
        {
            Invoke((MethodInvoker)delegate
            {
                //Update label, treating disposed specially
                if (state == LWRPState.DISPOSED)
                {
                    //Update status
                    statusLabel.Text = "DISCONNECTED";
                    statusLabel.BackColor = Color.FromArgb(255, 128, 128);
                    btnConnect.Enabled = true;

                    //Reset GUI
                    ClearGui();
                } else
                {
                    statusLabel.Text = state.ToString();
                    switch (state)
                    {
                        case LWRPState.PRE_INIT: statusLabel.BackColor = Color.FromArgb(255, 128, 128); break;
                        case LWRPState.CONNECTING: statusLabel.BackColor = Color.FromArgb(242, 177, 80); break;
                        case LWRPState.INITIALIZING: statusLabel.BackColor = Color.FromArgb(80, 193, 242); break;
                        case LWRPState.READY: statusLabel.BackColor = Color.FromArgb(80, 242, 80); break;
                    }
                }
            });
        }

        /// <summary>
        /// Utility function that will display a tab as ready when a task completes.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="page"></param>
        private void BindTabReady(Task task, TabPage page)
        {
            task.ContinueWith((Task t) =>
            {
                Invoke((MethodInvoker)delegate
                {
                    page.Text += "*";
                });
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Attempt to load last filename from disk
            if (File.Exists(LAST_IP_FILENAME))
                ipBox.Text = File.ReadAllText(LAST_IP_FILENAME);

            //Reset GUI
            ClearGui();

            //Start meter refresher
            meterRefreshTimer = new Timer();
            meterRefreshTimer.Tick += MeterRefreshTimer_Tick;
            meterRefreshTimer.Interval = 100;
            meterRefreshTimer.Start();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            btnApply.Enabled = false;
            Task<int> update = conn.SendUpdatesAsync();
            update.ContinueWith((Task<int> task) =>
            {
                Invoke((MethodInvoker)delegate
                {
                    if (task.Status == TaskStatus.RanToCompletion)
                    {

                    }
                    else if (task.Status == TaskStatus.Faulted)
                    {
                        Exception ex = task.Exception;
                        MessageBox.Show($"Failed to send updates: {ex.Message}{ex.StackTrace}", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    btnApply.Enabled = true;
                });
            });
        }

        private void ClearGui()
        {
            //Clear info
            infoDeviceModel.Text = "";
            infoDeviceName.Text = "";
            infoDeviceVer.Text = "";
            infoLwrpVer.Text = "";
            infoSrcNum.Text = "";
            infoDstNum.Text = "";
            infoGpiNum.Text = "";
            infoGpoNum.Text = "";

            //Clear sources and destinations
            sourcesPanel.Controls.Clear();
            dstPanel.Controls.Clear();
            gpiTable.ClearRows();
            gpoTable.ClearRows();
            meterTable.Controls.Clear();
            meterChannelsI.Clear();
            meterChannelsO.Clear();

            //Reset readiness marks
            foreach (var t in tabControl.TabPages)
                ((TabPage)t).Text = ((TabPage)t).Text.TrimEnd('*');
        }

        #region METERING

        private int meterConfiguredColumns;
        private Dictionary<int, VuMeter> meterChannelsI = new Dictionary<int, VuMeter>();
        private Dictionary<int, VuMeter> meterChannelsO = new Dictionary<int, VuMeter>();
        private Timer meterRefreshTimer;
        private Task lastRefreshTask;

        private void MeterRefreshTimer_Tick(object sender, EventArgs e)
        {
            if (checkAutoMeterRefresh.Checked && conn != null && (lastRefreshTask == null || lastRefreshTask.IsCompleted))
                lastRefreshTask = conn.RequestMetersAsync();
        }

        private void btnMeterRefresh_Click(object sender, EventArgs e)
        {
            if (lastRefreshTask == null || lastRefreshTask.IsCompleted)
                lastRefreshTask = conn.RequestMetersAsync();
        }

        private void Conn_OnMeterLevelsUpdated(LWRPConnection conn, Entities.MeterChannelReading[] ich, Entities.MeterChannelReading[] och)
        {
            Invoke((MethodInvoker)delegate
            {
                //Find the max channel required
                int maxChannel = 0;
                foreach (var c in ich)
                    maxChannel = Math.Max(maxChannel, c.Index);
                foreach (var c in och)
                    maxChannel = Math.Max(maxChannel, c.Index);

                //Make sure columns are created up to this
                meterTable.ColumnCount = maxChannel;
                while (meterConfiguredColumns < maxChannel)
                {
                    if (meterConfiguredColumns >= meterTable.ColumnStyles.Count)
                        meterTable.ColumnStyles.Add(new ColumnStyle());
                    meterTable.ColumnStyles[meterConfiguredColumns].Width = 60;
                    meterTable.ColumnStyles[meterConfiguredColumns].SizeType = SizeType.Absolute;
                    meterConfiguredColumns++;
                }

                //Set
                SetMeters(meterChannelsI, ich, "I-", 0);
                SetMeters(meterChannelsO, och, "O-", 2);
            });
        }

        private void SetMeters(Dictionary<int, VuMeter> meters, MeterChannelReading[] readings, string prefix, int rowOffset)
        {
            //Loop through readings
            foreach (var r in readings)
            {
                //Get index from 0
                int index = r.Index - 1;

                //Check if it's been created
                VuMeter meter;
                if (!meters.TryGetValue(index, out meter))
                {
                    //Create VU meter
                    meter = new VuMeter
                    {
                        Dock = DockStyle.Fill,
                        MinValue = -100,
                        MaxValue = 0,
                        ValueL = -100,
                        ValueR = -100,
                        ForeColor = Color.FromArgb(68, 120, 242)
                    };
                    meterTable.Controls.Add(meter, index, rowOffset);

                    //Create textbox
                    meterTable.Controls.Add(new Label
                    {
                        Text = prefix + r.Index.ToString(),
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleCenter,
                        AutoSize = true
                    }, index, rowOffset + 1);

                    //Add
                    meters.Add(index, meter);
                }

                //Update
                meter.ValueL = readings[index].Peek.L;
                meter.ValueR = readings[index].Peek.R;
            }
        }

        #endregion
    }
}