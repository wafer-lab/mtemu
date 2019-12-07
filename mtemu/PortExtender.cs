using System;
using System.Management;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace mtemu
{
    public class PortExtender
    {
        public PortExtender()
        {

        }

        public PortExtender(EventArrivedEventHandler deviceRemovedHandler)
        {
            DeviceRemovedHandler_ = deviceRemovedHandler;
        }

        private const byte CMD_SERIAL_GET = 0x03;
        private const byte CMD_PORT_WRITE = 0x02;
        private const byte CMD_PORT_READ = 0x01;
        private const byte CMD_RESPONSE = 0x00;
        private const byte CMD_MASK = 0x03;
        private const byte CMD_DATA_LEN_MASK = 0x1F;
        private const byte CMD_SHIFT = 0x05;

        private const byte CMD_MARKER_START = 0x00;
        private const byte CMD_MARKER_END = 0x04;
        private const byte CMD_MARKER_MASK = 0x04;

        private const byte SERIAL_LENGTH = 16;

        private const byte CMD_REQUEST_LENGTH = 0x02;
        private const byte CMD_REQUEST_PORT_WRITE_LENGTH = CMD_REQUEST_LENGTH + 2;
        private const byte CMD_REQUEST_PORT_READ_LENGTH = CMD_REQUEST_LENGTH + 1;
        private const byte CMD_REQUEST_SERIAL_GET_LENGTH = CMD_REQUEST_LENGTH;

        private const byte CMD_RESPONSE_LENGTH = 0x02;
        private const byte CMD_RESPONSE_PORT_WRITE_LENGTH = CMD_RESPONSE_LENGTH + 1;
        private const byte CMD_RESPONSE_PORT_READ_LENGTH = CMD_RESPONSE_LENGTH + 2;
        private const byte CMD_RESPONSE_SERIAL_GET_LENGTH = CMD_RESPONSE_LENGTH + SERIAL_LENGTH;

        public enum InPort : byte
        {
            PORT2_4_LOW = (2 << 2) | 1,
            PORT2_4_HIGH = (2 << 2) | 2,
            PORT2_8 = (2 << 2) | 3,

            PORT3_4_LOW = (3 << 2) | 1,
            PORT3_4_HIGH = (3 << 2) | 2,
            PORT3_8 = (3 << 2) | 3,

            PORT_UNKNOWN = 255,
        }

        public enum OutPort : byte
        {
            PORT0_4_LOW = (0 << 2) | 1,
            PORT0_4_HIGH = (0 << 2) | 2,
            PORT0_8 = (0 << 2) | 3,

            PORT1_4_LOW = (1 << 2) | 1,
            PORT1_4_HIGH = (1 << 2) | 2,
            PORT1_8 = (1 << 2) | 3,

            PORT_UNKNOWN = 255,
        }

        public struct DeviceInfo
        {
            public string com_id;
            public string serial;

            public DeviceInfo(string com_id = null, string serial = null)
            {
                this.com_id = com_id;
                this.serial = serial;
            }

            public DeviceInfo(DeviceInfo devInfo)
            {
                this.com_id = devInfo.com_id;
                this.serial = devInfo.serial;
            }
        }

        private DeviceInfo currentDeviceInfo_ = new DeviceInfo();
        private SerialPort deviceComPort_ = new SerialPort();
        private bool isDeviceOpened_ = false;


        private static List<string> GetComDevices()
        {
            ManagementObjectCollection ManObjReturn;
            ManagementObjectSearcher ManObjSearch;
            ManObjSearch = new ManagementObjectSearcher("Select * from Win32_SerialPort");
            ManObjReturn = ManObjSearch.Get();

            List<string> devices = new List<string>();

            foreach (ManagementObject ManObj in ManObjReturn)
            {
                var pnpDeviceId = ManObj["PNPDeviceID"].ToString();

                if (pnpDeviceId.Contains("USB\\VID_0483&PID_5740"))
                {
                    var deviceId = ManObj["DeviceID"].ToString();
                    devices.Add(deviceId);
                }
            }

            return devices;
        }

        public static DeviceInfo[] GetAvailableDevices()
        {
            var comDevices = GetComDevices();

            List<DeviceInfo> devices = new List<DeviceInfo>();
            foreach (var comDevice in comDevices)
            {
                string serial;

                if (CheckDeviceIsValid(comDevice, out serial))
                {
                    devices.Add(
                            new DeviceInfo(
                                comDevice,
                                serial));
                }
            }

            return devices.ToArray();
        }

        public bool SelectDevice(DeviceInfo deviceInfo)
        {
            if (deviceInfo.com_id == null)
                return false;

            if (deviceComPort_.IsOpen)
                deviceComPort_.Close();

            currentDeviceInfo_ = deviceInfo;
            deviceComPort_.BaudRate = 115200;
            deviceComPort_.DataBits = 8;
            deviceComPort_.Parity = Parity.None;
            deviceComPort_.PortName = deviceInfo.com_id;
            deviceComPort_.StopBits = StopBits.One;
            deviceComPort_.Open();

            isDeviceOpened_ = true;

            return true;
        }

        public void ReopenLastDevice()
        {
            if (deviceComPort_.IsOpen)
                deviceComPort_.Close();

            deviceComPort_.Open();
        }

        public bool IsDeviceOpened()
        {
            return isDeviceOpened_;
        }

        public void CloseDevice()
        {
            if (deviceComPort_.IsOpen)
                deviceComPort_.Close();

            isDeviceOpened_ = false;
        }

        private static ManagementEventWatcher removal_;

        private EventArrivedEventHandler DeviceRemovedHandler_;

        public bool CheckDeviceRemoved()
        {
            var comDevices = GetComDevices();

            foreach (var device in comDevices)
            {
                if (device == currentDeviceInfo_.com_id)
                    return false;
            }

            return true;
        }

        public void StatusPoll()
        {
            const string unpluggedSql = "SELECT * FROM __InstanceDeletionEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_PnPEntity'";
            var scope = new ManagementScope("root\\CIMV2") { Options = { EnablePrivileges = true } };
            var unPluggedQuery = new WqlEventQuery(unpluggedSql);
            removal_ = new ManagementEventWatcher(scope, unPluggedQuery);
            removal_.EventArrived += DeviceRemovedHandler_;
            removal_.Start();
        }

        private static bool CheckDeviceIsValid(string deviceId, out string serial_str)
        {
            SerialPort sp = new SerialPort(
                deviceId,
                115200,
                Parity.None,
                8,
                StopBits.One);

            serial_str = null;

            if (!sp.IsOpen)
                sp.Open();

            byte[] req_buf = new byte[CMD_REQUEST_SERIAL_GET_LENGTH];
            byte[] data_buf = null;
            byte data_size = 0;
            object res;

            bool result = false;

            PrepareRequestBuf(ref req_buf, CMD_REQUEST_SERIAL_GET_LENGTH, data_buf, data_size, CMD_SERIAL_GET);
            try
            {
                CmdSendRecv(ref sp, req_buf, out res);

                byte[] serial = (byte[])res;

                if (serial != null && serial.Length >= SERIAL_LENGTH)
                {
                    if (serial[0] == 'T' &&
                        serial[1] == 'M' &&
                        serial[SERIAL_LENGTH - 2] == 'E' &&
                        serial[SERIAL_LENGTH - 1] == 'P')
                    {
                        serial_str = BitConverter.ToString(serial, 2, 12).Replace("-", "");
                        result = true;
                    }
                }
            }
            catch (TimeoutException e)
            {
                result = false;
            }
            finally
            {
                sp.Close();
            }

            return result;
        }

        private static ManualResetEvent mre_ = new ManualResetEvent(false);

        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            mre_.Set();
        }

        private static bool ParseResponse(ref byte[] buf, out byte[] resp_data)
        {
            resp_data = null;

            byte resp_start = buf[0];
            if (((resp_start >> CMD_SHIFT) & CMD_MARKER_MASK) == CMD_MARKER_START)
            {
                if (((resp_start >> CMD_SHIFT) & CMD_MASK) == CMD_RESPONSE)
                {
                    byte resp_size = Convert.ToByte(resp_start & CMD_DATA_LEN_MASK);
                    byte resp_end = buf[resp_size + 1];

                    if (((resp_end >> CMD_SHIFT) & CMD_MARKER_MASK) == CMD_MARKER_END)
                    {
                        if ((resp_end & CMD_DATA_LEN_MASK) == resp_size)
                        {
                            resp_data = new byte[resp_size];
                            Array.Copy(buf, 1, resp_data, 0, resp_size);

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static void CmdSendRecv(ref SerialPort dev, byte[] in_buf, out object res)
        {
            res = null;

            if (dev.IsOpen)
            {
                mre_.Reset();
                dev.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);

                dev.Write(in_buf, 0, in_buf.Length);

                var signal = mre_.WaitOne(1000, false);
                dev.DataReceived -= new SerialDataReceivedEventHandler(SerialPort_DataReceived);

                if (signal)
                {
                    var buf_size = dev.ReadBufferSize;
                    if (buf_size > 0)
                    {
                        byte[] buf = new byte[buf_size];
                        dev.Read(buf, 0, buf_size);

                        byte[] resp_data;

                        ParseResponse(ref buf, out resp_data);
                        res = resp_data;

                        if (resp_data == null ||
                            ((resp_data.Length == 1) && (Convert.ToSByte(resp_data[0]) < 0))
                            )
                            throw new ArgumentException("Device returned request failed");
                    }
                    else
                        throw new ArgumentNullException("Received buffer size is zero");
                }
                else
                    throw new TimeoutException("Device timeout occured");
            }
            else
                throw new AccessViolationException("MTPE is not connected");
        }

        private static void PrepareRequestBuf(ref byte[] req_buf, byte buf_size, byte[] data_buf, byte data_size, byte cmd)
        {
            req_buf[0] = Convert.ToByte(((cmd | CMD_MARKER_START) << CMD_SHIFT) | data_size);
            req_buf[buf_size - 1] = Convert.ToByte(((cmd | CMD_MARKER_END) << CMD_SHIFT) | data_size);

            for (byte i = 0; i < data_size; i++)
            {
                req_buf[i + 1] = data_buf[i];
            }
        }

        public void WritePort(OutPort outPort, DataPointerType pointerType, byte val)
        {
            if (deviceComPort_ == null || !deviceComPort_.IsOpen)
            {
                System.Windows.Forms.MessageBox.Show(
                    "Внешнее устройство не подключено",
                    "Ошибка!",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);

                return;
            }

            byte[] req_buf = new byte[CMD_REQUEST_PORT_WRITE_LENGTH];
            byte[] data_buf = { (byte)outPort, val };
            byte data_size = (byte)data_buf.Length;
            object res;

            PrepareRequestBuf(ref req_buf, CMD_REQUEST_PORT_WRITE_LENGTH, data_buf, data_size, CMD_PORT_WRITE);
            CmdSendRecv(ref deviceComPort_, req_buf, out res);
        }

        public byte ReadPort(InPort inPort, DataPointerType pointerType)
        {
            if (deviceComPort_ == null || !deviceComPort_.IsOpen)
            {
                System.Windows.Forms.MessageBox.Show(
                    "Внешнее устройство не подключено",
                    "Ошибка!",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);

                return 0;
            }

            byte[] req_buf = new byte[CMD_REQUEST_PORT_READ_LENGTH];
            byte[] data_buf = { (byte)inPort };
            byte data_size = (byte)data_buf.Length;
            object res;

            PrepareRequestBuf(ref req_buf, CMD_REQUEST_PORT_READ_LENGTH, data_buf, data_size, CMD_PORT_READ);
            CmdSendRecv(ref deviceComPort_, req_buf, out res);

            if (res == null)
                throw new ArgumentNullException("received value is null");

            byte[] buf = (byte[])res;
            var port = buf[0] & CMD_DATA_LEN_MASK;
            if (port != (byte)inPort)
                throw new ArgumentException("response port is invalid");

            byte val = buf[1];

            return val;
        }

        public string SerialGet()
        {
            byte[] req_buf = new byte[CMD_REQUEST_SERIAL_GET_LENGTH];
            byte[] data_buf = null;
            byte data_size = 0;
            object res;

            PrepareRequestBuf(ref req_buf, CMD_REQUEST_SERIAL_GET_LENGTH, data_buf, data_size, CMD_SERIAL_GET);
            CmdSendRecv(ref deviceComPort_, req_buf, out res);

            string serial = (string)res;

            return serial;
        }
    }
}
