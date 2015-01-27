using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using PAARC.Communication;
using PAARC.Shared;
using PAARC.Shared.ControlCommands;
using PAARC.Shared.Data;

namespace ConsoleApplication
{
    class Program
    {
        private static IPAddress _serverAddress;

        static void Main(string[] args)
        {
            // output the help screen
            PrintHelp();

            // get the first IP address
            var addresses = Dns.GetHostAddresses(string.Empty);
            _serverAddress = addresses.First(o => o.AddressFamily == AddressFamily.InterNetwork);

            // create the controller
            PhoneControllerServer server = new PhoneControllerServer();
            server.Error += Server_Error;
            server.StateChanged += Server_StateChanged;
            server.ControlCommandSent += Server_ControlCommandSent;
            server.DataMessageReceived += Server_DataMessageReceived;
            Console.WriteLine("Server created.");

            // initialize with the local address
            server.Initialize(_serverAddress);

            string command = string.Empty;
            while (command != "q")
            {
                command = Console.ReadLine();

                try
                {
                    if (command == "h")
                    {
                        PrintHelp();
                    }
                    else if (command.StartsWith("a") && command.Length > 1)
                    {
                        if (command[1] == '1')
                        {
                            Console.WriteLine("Sending accelerometer start command...");
                            var msg = ControlCommandFactory.CreateCommand(DataType.Accelerometer, ControlCommandAction.Start);
                            server.SendCommandAsync(msg);
                        }
                        else if (command[1] == '0')
                        {
                            Console.WriteLine("Sending accelerometer stop command...");
                            var msg = ControlCommandFactory.CreateCommand(DataType.Accelerometer, ControlCommandAction.Stop);
                            server.SendCommandAsync(msg);
                        }
                    }
                    else if (command == "i")
                    {
                        Console.WriteLine("Trying to initialize server...");
                        server.Initialize(_serverAddress);
                    }
                    else if (command.StartsWith("m") && command.Length > 1)
                    {
                        int milliseconds = 0;
                        var substring = command.Substring(1);
                        if (int.TryParse(substring, out milliseconds))
                        {
                            Console.WriteLine("Sending configuration for MinMillisecondsBetweenMessages...");
                            var msg = ControlCommandFactory.CreateConfigurationCommand();
                            msg.Configuration.MinMillisecondsBetweenMessages = milliseconds;
                            server.SendCommandAsync(msg);
                        }
                    }
                    else if (command == "s")
                    {
                        Console.WriteLine("Shutting down server...");
                        server.Shutdown();
                    }
                    else if (command.StartsWith("txt") && command.Length > 3)
                    {
                        if (command[3] == '1')
                        {
                            Console.WriteLine("Sending text start command...");
                            var msg = ControlCommandFactory.CreateCommand(DataType.Text, ControlCommandAction.Start);
                            server.SendCommandAsync(msg);
                        }
                        else if (command[3] == '0')
                        {
                            Console.WriteLine("Sending text stop command...");
                            var msg = ControlCommandFactory.CreateCommand(DataType.Text, ControlCommandAction.Stop);
                            server.SendCommandAsync(msg);
                        }
                    }
                    else if (command.StartsWith("t") && command.Length > 1)
                    {
                        if (command[1] == '1')
                        {
                            Console.WriteLine("Sending raw touch start command...");
                            var msg = ControlCommandFactory.CreateCommand(DataType.Touch, ControlCommandAction.Start);
                            server.SendCommandAsync(msg);
                        }
                        else if (command[1] == '0')
                        {
                            Console.WriteLine("Sending raw touch stop command...");
                            var msg = ControlCommandFactory.CreateCommand(DataType.Touch, ControlCommandAction.Stop);
                            server.SendCommandAsync(msg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An exception occurred: ");
                    Console.WriteLine(ex.ToString());
                }
            }

            // clean up
            server.Shutdown();
        }

        private static void PrintHelp()
        {
            Console.WriteLine();
            Console.WriteLine("You can use the following commands for testing:");
            Console.WriteLine("a[0|1]\t\tstop|start the acquisition of accelerometer data");
            Console.WriteLine("i\t\t(re-)initialize the server (e.g. after shut down)");
            Console.WriteLine("m[nnnn]\t\tsets the min number of milliseconds between messages to nnnn");
            Console.WriteLine("q\t\tshuts down the server and quits the application");
            Console.WriteLine("s\t\tshut down the server without quitting the application");
            Console.WriteLine("t[0|1]\t\tstop|start the acquisition of raw touch data");
            Console.WriteLine("txt[0|1]\tstop|start the acquisition of text data");
            Console.WriteLine();
            Console.WriteLine("Use 'h' to show this help screen again.");
            Console.WriteLine();
        }

        static void Server_ControlCommandSent(object sender, EventArgs e)
        {
            Console.WriteLine("Control command sent.");
        }

        static void Server_StateChanged(object sender, PhoneControllerStateEventArgs e)
        {
            Console.WriteLine("Server changed state to " + e.State);
        }

        static void Server_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("*** ERROR: " + e.Error.Message + ", " + e.Error.ErrorCode ?? "[No error code given]");
            Console.WriteLine("Initiating server shutdown...");

            var server = sender as PhoneControllerServer;
            if (server != null)
            {
                server.Shutdown();
            }
        }

        static void Server_DataMessageReceived(object sender, DataMessageEventArgs e)
        {
            Console.WriteLine("Data received...");

            var data = e.DataMessage;
            switch (data.DataType)
            {
                case DataType.ControllerInfo:
                    HandleControllerInfo(data as ControllerInfoData);
                    break;
                case DataType.Accelerometer:
                    HandleAccelerometer(data as AccelerometerData);
                    break;
                case DataType.Touch:
                    HandleTouch(data as TouchData);
                    break;
                case DataType.Text:
                    HandleText(data as TextData);
                    break;
            }
        }

        private static void HandleControllerInfo(ControllerInfoData controllerInfoData)
        {
            string template = "Controller info: {0}Version: {1}{2}IsTouchSupported: {3}{4}IsAccelerometerSupported: {5}{6}IsGyroscopeSupported: {7}{8}IsCompassSupported: {9}{10}IsMotionSupported: {11}{12}DisplayWidth: {13}{14}DisplayHeight: {15}";
            string message = string.Format(template,
                                           Environment.NewLine,
                                           controllerInfoData.ClientVersion, Environment.NewLine,
                                           controllerInfoData.IsTouchSupported, Environment.NewLine,
                                           controllerInfoData.IsAccelerometerSupported, Environment.NewLine,
                                           controllerInfoData.IsGyroscopeSupported, Environment.NewLine,
                                           controllerInfoData.IsCompassSupported, Environment.NewLine,
                                           controllerInfoData.IsMotionSupported, Environment.NewLine,
                                           controllerInfoData.DisplayWidth, Environment.NewLine,
                                           controllerInfoData.DisplayHeight);
            Console.WriteLine(message);
        }

        private static void HandleAccelerometer(AccelerometerData accelerometerData)
        {
            const string template = "Accelerometer ({0}.{1}) Timestamp={2} X={3} Y={4} Z={5}";
            string message = string.Format(template,
                DateTime.Now.Second.ToString("00"),
                DateTime.Now.Millisecond.ToString("000"),
                accelerometerData.Timestamp.Ticks,
                accelerometerData.X.ToString("0.000"),
                accelerometerData.Y.ToString("0.000"),
                accelerometerData.Z.ToString("0.000"));
            Console.WriteLine(message);
        }

        private static void HandleTouch(TouchData touchData)
        {
            const string template = "Touch ({0}.{1}) Touch points={2} First touch point X={3} Y={4} State={5}";
            string message = string.Format(template,
                DateTime.Now.Second.ToString("00"),
                DateTime.Now.Millisecond.ToString("000"),
                touchData.TouchPoints.Count,
                touchData.TouchPoints[0].Location.X.ToString("0.000"),
                touchData.TouchPoints[0].Location.Y.ToString("0.000"),
                touchData.TouchPoints[0].State.ToString());
            Console.WriteLine(message);
        }

        private static void HandleText(TextData textData)
        {
            const string template = "Text ({0}.{1}) \"{2}\"";
            string message = string.Format(template,
                DateTime.Now.Second.ToString("00"),
                DateTime.Now.Millisecond.ToString("000"),
                textData.Text);
            Console.WriteLine(message);
        }
    }
}
