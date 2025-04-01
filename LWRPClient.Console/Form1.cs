using LWRPClient.Console.Panels;
using LWRPClient.Console.Properties;
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
                conn = new LWRPConnection(transport, LWRPEnabledFeature.SOURCES | LWRPEnabledFeature.DESTINATIONS);
                conn.OnInfoDataReceived += Conn_OnInfoDataReceived;
                conn.Sources.OnBatchUpdate += Conn_OnSrcBatchUpdate;
                conn.Destinations.OnBatchUpdate += Conn_OnDstBatchUpdate;
                conn.OnConnectionStateUpdate += Conn_OnConnectionStateUpdate;
                conn.Initialize();

                //Set button text
                btnConnect.Text = "Disconnect";
            } else // Disconnect mode
            {
                //Set button text and state
                btnConnect.Enabled = false;
                btnConnect.Text = "Connect";

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

        private void Form1_Load(object sender, EventArgs e)
        {
            //Attempt to load last filename from disk
            if (File.Exists(LAST_IP_FILENAME))
                ipBox.Text = File.ReadAllText(LAST_IP_FILENAME);

            //Reset GUI
            ClearGui();
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
        }
    }
}