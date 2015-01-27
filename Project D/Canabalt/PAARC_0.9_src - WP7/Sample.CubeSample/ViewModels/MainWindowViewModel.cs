using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Media.Media3D;
using PAARC.Communication;
using PAARC.Shared;
using PAARC.Shared.ControlCommands;
using PAARC.Shared.Data;

namespace Cube.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private double _xAxisRotation;
        private double _yAxisRotation;
        private Point3D _cameraPosition;
        private double _xTranslation;
        private double _yTranslation;
        private double _zTranslation;

        private readonly PhoneControllerServer _phoneController;

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public double XAxisRotation
        {
            get
            {
                return _xAxisRotation;
            }
            set
            {
                if (_xAxisRotation != value)
                {
                    _xAxisRotation = value;
                    RaisePropertyChanged("XAxisRotation");
                }
            }
        }

        public double YAxisRotation
        {
            get
            {
                return _yAxisRotation;
            }
            set
            {
                if (_yAxisRotation != value)
                {
                    _yAxisRotation = value;
                    RaisePropertyChanged("YAxisRotation");
                }
            }
        }

        public double XTranslation
        {
            get
            {
                return _xTranslation;
            }
            set
            {
                if (_xTranslation != value)
                {
                    _xTranslation = value;
                    RaisePropertyChanged("XTranslation");
                }
            }
        }

        public double YTranslation
        {
            get
            {
                return _yTranslation;
            }
            set
            {
                if (_yTranslation != value)
                {
                    _yTranslation = value;
                    RaisePropertyChanged("YTranslation");
                }
            }
        }

        public double ZTranslation
        {
            get
            {
                return _zTranslation;
            }
            set
            {
                if (_zTranslation != value)
                {
                    _zTranslation = value;
                    RaisePropertyChanged("ZTranslation");
                }
            }
        }

        public Point3D CameraPosition
        {
            get
            {
                return _cameraPosition;
            }
            set
            {
                if (_cameraPosition != value)
                {
                    _cameraPosition = value;
                    RaisePropertyChanged("CameraPosition");
                }
            }
        }

        public MainWindowViewModel()
        {
            // default values
            YAxisRotation = 40.0;
            CameraPosition = new Point3D(0.0, 0.0, 5.0);

            // initialize controller
            _phoneController = new PhoneControllerServer();
            _phoneController.Error += PhoneController_Error;
            _phoneController.StateChanged += PhoneController_StateChanged;
            _phoneController.DataMessageReceived += PhoneController_DataMessageReceived;
        }

        private void PhoneController_Error(object sender, ErrorEventArgs e)
        {
            // do something more meaningful than this:
            MessageBox.Show("An error occurred: " + e.Error.Message);

            // shut down the controller
            // => this will trigger a re-initialization attempt when the controller state changes to "Closed" (see below)
            _phoneController.Shutdown();
        }

        private void PhoneController_StateChanged(object sender, PhoneControllerStateEventArgs e)
        {
            // decide what to do
            switch (e.State)
            {
                case PhoneControllerState.Closed:
                    // simply try to initialize
                    InitializeController();
                    break;
            }
        }

        public void InitializeController()
        {
            try
            {
                var addresses = Dns.GetHostAddresses(string.Empty);
                var address = addresses.First(o => o.AddressFamily == AddressFamily.InterNetwork);
                _phoneController.Initialize(address);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred while initializing the controller: " + ex);
            }
        }

        private void ConfigureController()
        {
            if (_phoneController != null && _phoneController.State == PhoneControllerState.Ready)
            {
                // you can send configuration data remotely to the phone to set up basic parameters
                var configuration = ControlCommandFactory.CreateConfigurationCommand();

                // tells the phone to try and reconnect to the server after
                // reactivation (e.g. after tombstoning or after the lock screen becomes active).
                configuration.Configuration.AutoReconnectOnActivation = true;

                _phoneController.SendCommandAsync(configuration);

                // send accelerometer start command (accelerometer is supported on all devices, but check to be sure)
                if (_supportsAccelerometer)
                {
                    SendStartControlCommand(DataType.Accelerometer);
                }

                // send all touch commands (touch is supported on all devices, but check to be sure)
                if (_supportsTouch)
                {
                    SendStartControlCommand(DataType.Touch);
                    SendStartControlCommand(DataType.Pinch);
                    SendStartControlCommand(DataType.PinchComplete);
                }
            }
        }

        private void SendStartControlCommand(DataType dataType)
        {
            var command = ControlCommandFactory.CreateCommand(dataType, ControlCommandAction.Start);
            _phoneController.SendCommandAsync(command);
        }

        #region Controller Data Reception

        private bool _supportsAccelerometer;
        private bool _supportsTouch;
        private double _lastTouchX;
        private double _lastTouchY;
        private double? _lastPinchDistance;

        private void PhoneController_DataMessageReceived(object sender, DataMessageEventArgs e)
        {
            // we have received data from the controller
            switch (e.DataMessage.DataType)
            {
                case DataType.ControllerInfo:
                    // this info is received every time the controller connects
                    // => a good place to get the device caps and trigger configuration
                    var controllerInfo = e.DataMessage as ControllerInfoData;
                    _supportsAccelerometer = controllerInfo.IsAccelerometerSupported;
                    _supportsTouch = controllerInfo.IsTouchSupported;
                    ConfigureController();
                    break;
                case DataType.Accelerometer:
                    // this is accelerometer data received from the phone
                    // => we are going to rotate the model based on that information
                    HandleAccelerometerData(e.DataMessage as AccelerometerData);
                    break;
                case DataType.Touch:
                    // touch data moves the object around on the screen
                    HandleTouchData(e.DataMessage as TouchData);
                    break;
                case DataType.Pinch:
                    // pinching translates the object into and out of the screen ("zoom")
                    HandlePinchData(e.DataMessage as PinchData);
                    break;
                case DataType.PinchComplete:
                    // used to determine when a pinch gesture is complete (so we can cleanly 
                    // start over when the next new pinch gesture is reported
                    HandlePinchCompleteData(e.DataMessage as PinchCompleteData);
                    break;
            }
        }

        private void HandleAccelerometerData(AccelerometerData accelerometerData)
        {
            YAxisRotation -= accelerometerData.Y * 10.0;
        }

        private void HandleTouchData(TouchData touchData)
        {
            if (touchData.TouchPoints.Count > 1)
            {
                // we only process this if the user uses one finger only
                return;
            }

            // use simple restriction values to avoid that the user moves the cube off screen
            const double XTranslationMaximum = 3.0;
            const double YTranslationMaximum = 3.0;

            // get the touch point and inspect what happened
            var touchPoint = touchData.TouchPoints[0];
            switch (touchPoint.State)
            {
                case TouchPointState.Invalid:
                case TouchPointState.Released:
                    // nothing to do
                    return;
                case TouchPointState.Pressed:
                    // store a new reference
                    _lastTouchX = touchPoint.Location.X;
                    _lastTouchY = touchPoint.Location.Y;
                    break;
                case TouchPointState.Moved:
                    // calculate the change
                    var changeX = (_lastTouchX - touchPoint.Location.X) * -0.01;
                    var changeY = (_lastTouchY - touchPoint.Location.Y) * 0.01;

                    // apply to translations
                    XTranslation = Math.Min(XTranslationMaximum, Math.Max(-XTranslationMaximum, XTranslation + changeX));
                    YTranslation = Math.Min(YTranslationMaximum, Math.Max(-YTranslationMaximum, YTranslation + changeY));

                    // new references
                    _lastTouchX = touchPoint.Location.X;
                    _lastTouchY = touchPoint.Location.Y;
                    break;
            }
        }

        private void HandlePinchData(PinchData pinchData)
        {
            if (_lastPinchDistance == null)
            {
                _lastPinchDistance = CalculateDistance(pinchData.TouchPoint.Location, pinchData.TouchPoint2.Location);
                return;
            }

            // calculate new distance
            var newDistance = CalculateDistance(pinchData.TouchPoint.Location, pinchData.TouchPoint2.Location);

            // use difference for z-translation
            const double ZTranslationMinimum = -10.0;
            const double ZTranslationMaximum = 0.0;
            var difference = (newDistance - _lastPinchDistance) * 0.01;
            ZTranslation = Math.Min(ZTranslationMaximum, Math.Max(ZTranslationMinimum, ZTranslation + difference.Value));

            // new reference
            _lastPinchDistance = newDistance;
        }

        private double? CalculateDistance(Vector2 first, Vector2 second)
        {
            var xDiff = first.X - second.X;
            var yDiff = first.Y - second.Y;

            var length = Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
            return length;
        }

        private void HandlePinchCompleteData(PinchCompleteData pinchCompleteData)
        {
            // simply reset the pinch reference value
            // so we can start a new one in the HandlePinchData method if necessary
            _lastPinchDistance = null;
        }

        #endregion

        private void RaisePropertyChanged(string propertyName)
        {
            var handlers = PropertyChanged;
            if (handlers != null)
            {
                handlers(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Shutdown()
        {
            // called when the application closes
            if (_phoneController != null)
            {
                _phoneController.Error -= PhoneController_Error;
                _phoneController.StateChanged -= PhoneController_StateChanged;
                _phoneController.DataMessageReceived -= PhoneController_DataMessageReceived;

                _phoneController.Shutdown();
            }
        }
    }
}
