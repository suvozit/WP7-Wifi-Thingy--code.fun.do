using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using PAARC.Shared;
using PAARC.Shared.Data;
using System.Runtime.InteropServices;
using WindowsInput;

namespace PCController.Input
{
    /// <summary>
    /// Emulates mouse and keyboard input, based on what kind of data is received from a Windows Phone device.
    /// </summary>
    public class InputEmulator
    {
        // variables needed to process touch and drag data
        private double _controllerDisplayWidth;
        private double _controllerDisplayHeight;
        private float _lastTouchX = -1.0f;
        private float _lastTouchY = -1.0f;
        private double? _lastDragX;
        private double? _lastDragY;
        private bool _dragMouseButtonDownSend = false;
        private Process[] _processes;
        
        /// <summary>
        /// Gets or sets a value indicating whether the emulator should use relative mode
        /// for positioning of the mouse cursor based on touch input.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the emulator uses relative touch input; otherwise, <c>false</c>.
        /// </value>
        public bool UseRelativeTouchInput
        {
            get;
            set;
        }

        public InputEmulator()
        {            
            Process[] processes = System.Diagnostics.Process.GetProcessesByName("flashplayer_16_sa.exe");
        }

        /// <summary>
        /// Processes the specified data message and translates it to mouse or keyboard input.
        /// </summary>
        /// <param name="data">The data message to process.</param>
        public void Process(IDataMessage data)
        {
            // simply dispatch the data to the correct method
            switch (data.DataType)
            {
                case DataType.ControllerInfo:
                    Process(data as ControllerInfoData);
                    break;
                case DataType.Accelerometer:
                    Process(data as AccelerometerData);
                    break;
                case DataType.Touch:
                    Process(data as TouchData);
                    break;
                case DataType.Tap:
                    Process(data as TapData);
                    break;
                case DataType.DoubleTap:
                    Process(data as DoubleTapData);
                    break;
                case DataType.Hold:
                    Process(data as HoldData);
                    break;
                case DataType.Flick:
                    Process(data as FlickData);
                    break;
                case DataType.CustomDrag:
                    Process(data as CustomDragData);
                    break;
                case DataType.CustomDragComplete:
                    Process(data as CustomDragCompleteData);
                    break;
                case DataType.Text:
                    Process(data as TextData);
                    break;
            }
        }

        private void Process(ControllerInfoData data)
        {
            _controllerDisplayWidth = data.DisplayWidth;
            _controllerDisplayHeight = data.DisplayHeight;
        }

        const double alpha = 0.5;
        double fXg = 0;
        double fYg = 0;
        double fZg = 0;

        Vector2 GetRollPitch(AccelerometerData data)
        {
            //Low Pass Filter
            fXg = data.X * alpha + (fXg * (1.0 - alpha));
            fYg = data.Y * alpha + (fYg * (1.0 - alpha));
            fZg = data.Z * alpha + (fZg * (1.0 - alpha));

            //Roll & Pitch Equations
            Vector2 pitch_roll = new Vector2();
            float pitch = (float)((Math.Atan2(-fYg, fZg) * 180.0) / Math.PI);
            pitch_roll.Y = (float)((Math.Atan2(fXg, Math.Sqrt(fYg * fYg + fZg * fZg)) * 180.0) / Math.PI);
            pitch = (pitch >= 0) ? (180 - pitch) : (-pitch - 180);
            pitch_roll.X = pitch;

            return pitch_roll;
        }


        double ToDegrees(float rad)
        {
            return rad * (180/3.14);
        }

        static bool send_input = true;

        private void Process(AccelerometerData data)
        {
            // here is how it works:
            // we want the user to hold the phone in landscape mode
            // => x < 0 = cursor down
            //    x > 0 = cursor up
            //    y < 0 = cursor right
            //    y > 0 = cursor left
            
            Vector2 pitch_roll = GetRollPitch(data);

            var changeX = -data.Y * 10.0;
            var changeY = -data.X * 10.0;

            double xAngle = ToDegrees(data.X);
            double yAngle = ToDegrees(data.Y);
            double zAngle = ToDegrees(data.Z);
           // Console.WriteLine("X = "+ ToDegrees(data.X) + "Y = " + ToDegrees(data.Y) + "z =" + ToDegrees(data.Z)); 

            // move mouse position
            var currentPosition = Win32Wrapper.GetCursorPosition();
            var newPositionX = (int)Math.Round(currentPosition.X + changeX);
            var newPositionY = (int)Math.Round(currentPosition.Y + changeY);
            //Win32Wrapper.SetCursorPosition(newPositionX, newPositionY);            

            if(yAngle < -20 )
            {
                //Up
                if(send_input)
                {                    
                    Win32Wrapper.SendMouseLeftButtonClick();
                    send_input = false;
                }
            }            
            else if(yAngle > 20)
            {
                
                if(send_input)
                {
                    
                    send_input = false;
                }                
            }
            else if(xAngle > 30)
            {
                //Right
                if(send_input)
                {
                    InputSimulator.SimulateKeyPress(VirtualKeyCode.RIGHT);
                    send_input = false;
                }                
            }

            else if(xAngle < -30)
            {
                //Left
                if(send_input)
                {
                    InputSimulator.SimulateKeyPress(VirtualKeyCode.LEFT);
                    send_input = false;
                }
            }
            else
            {
                send_input = true;
            }
        }

        private void Process(TouchData data)
        {
            Debug.WriteLine("InputEmulator: Processing raw touch input");

            var touchPointCount = data.TouchPoints.Count;

            // should never happen
            if (touchPointCount < 1)
            {
                return;
            }

            // do not process the data if any of the points is invalid
            if (data.TouchPoints.Any(o => o.State == TouchPointState.Invalid))
            {
                return;
            }

            // was a touch point released? => reset the reference point and done
            if (data.TouchPoints.Any(o => o.State == TouchPointState.Released))
            {
                // reset the last references
                _lastTouchX = -1.0f;
                _lastTouchY = -1.0f;
                return;
            }

            // get the coordinates
            var x = data.TouchPoints.Sum(o => o.Location.X) / touchPointCount;
            var y = data.TouchPoints.Sum(o => o.Location.Y) / touchPointCount;

            // if any touch points are newly pressed, set new reference data
            if (data.TouchPoints.Any(o => o.State == TouchPointState.Pressed))
            {
                _lastTouchX = x;
                _lastTouchY = y;
                return;
            }

            // at this point we know the touch point(s) state('s) is 'Moved'

            // get current computer's virtual screen width
            var screenWidth = SystemInformation.VirtualScreen.Width;
            var screenHeight = SystemInformation.VirtualScreen.Height;

            // decide what to do
            if (touchPointCount > 1)
            {
                // the user uses more than one finger => we use this for two-finger scrolling,
                // by emulating mouse wheel data
                var changeY = y - _lastTouchY;

                // change sign because the wheel works "the other way round"
                changeY *= -1;

                // convert to wheel ticks
                var ticks = (int)Math.Round(changeY / 15.0);
                if (ticks == 0)
                {
                    // if the scrolling was too "slow" to cause any mouse wheel ticks
                    // we simply return (not storing new reference coordinates!).
                    // this allows "slow" scrolling because the next time this is invoked,
                    // the difference is computed with regards to the old reference,
                    // making a tick >= 1 more likely
                    return;
                }

                Win32Wrapper.SendMouseWheel(ticks);
            }
            else
            {
                // for only one finger touch data
                // we use the data for cursor movement
                if (UseRelativeTouchInput)
                {
                    // calc the change
                    var changeX = (int)Math.Ceiling(x - _lastTouchX);
                    var changeY = (int)Math.Ceiling(y - _lastTouchY);

                    if (changeX == 0 && changeY == 0)
                    {
                        // return without setting a new reference point (below)
                        // to allow values to aggregate, but do not use small values (in particular zero)
                        // to avoid rounding problems (=> "wandering" cursor)
                        return;
                    }

                    // move mouse position
                    var currentPosition = Win32Wrapper.GetCursorPosition();
                    var newPositionX = (int)Math.Round(currentPosition.X + changeX);
                    var newPositionY = (int)Math.Round(currentPosition.Y + changeY);

                    Win32Wrapper.SetCursorPosition(newPositionX, newPositionY);
                }
                else
                {
                    // calculate new position depending on the controller's screen dimension
                    var newX = (x / _controllerDisplayWidth) * screenWidth;
                    var newY = (y / _controllerDisplayHeight) * screenHeight;

                    // set absolute mouse position
                    var newPositionX = (int)Math.Round(newX) + SystemInformation.VirtualScreen.Left;
                    var newPositionY = (int)Math.Round(newY) + SystemInformation.VirtualScreen.Top;
                    Win32Wrapper.SetCursorPosition(newPositionX, newPositionY);
                }
            }

            // set new references
            _lastTouchX = x;
            _lastTouchY = y;
        }

        private void Process(TapData data)
        {
            // "click"
            Win32Wrapper.SendMouseLeftButtonClick();
        }

        private void Process(DoubleTapData data)
        {
            // "double-click"
            Win32Wrapper.SendMouseLeftButtonDoubleClick();
        }

        private void Process(HoldData data)
        {
            // we translate this to a right button click
            // to bring up context menus
            Win32Wrapper.SendMouseRightButtonClick();
        }

        private void Process(FlickData data)
        {
            // flicks are simply used to move the mouse cursor
            // by a large amount of distance quickly
            var changeX = data.Delta.X;
            var changeY = data.Delta.Y;

            // move mouse position
            var currentPosition = Win32Wrapper.GetCursorPosition();
            var newPositionX = (int)Math.Round(currentPosition.X + changeX);
            var newPositionY = (int)Math.Round(currentPosition.Y + changeY);

            Win32Wrapper.SetCursorPosition(newPositionX, newPositionY);
        }

        private void Process(CustomDragData data)
        {
            Debug.WriteLine("InputEmulator: Processing custom drag input");

            // send mouse down for the first touch point
            if (!_dragMouseButtonDownSend)
            {
                Debug.WriteLine("InputEmulator: Custom Drag: Left Button down");

                _dragMouseButtonDownSend = true;
                Win32Wrapper.SendMouseLeftButtonDown();
            }

            // get the touch point
            var touchPoint = data.TouchPoint;
            if (touchPoint.State == TouchPointState.Pressed)
            {
                // can happen multiple times when the user lifts their finger
                // and continues dragging at another place ("relative" dragging)
                _lastDragX = touchPoint.Location.X;
                _lastDragY = touchPoint.Location.Y;
            }
            else if (data.TouchPoint.State == TouchPointState.Moved && _lastDragX.HasValue && _lastDragY.HasValue)
            {
                Debug.WriteLine("InputEmulator: Custom Drag: Moving");

                // calc the change
                var changeX = (int)Math.Floor(touchPoint.Location.X - _lastDragX.Value);
                var changeY = (int)Math.Floor(touchPoint.Location.Y - _lastDragY.Value);

                if (changeX == 0 && changeY == 0)
                {
                    // return without setting a new reference point (below)
                    // to allow values to aggregate, but do not use small values (in particular zero)
                    // to avoid rounding problems (=> "wandering" cursor)
                    return;
                }

                // move mouse position
                var currentPosition = Win32Wrapper.GetCursorPosition();
                var newPositionX = (int)Math.Round(currentPosition.X + changeX);
                var newPositionY = (int)Math.Round(currentPosition.Y + changeY);

                Win32Wrapper.SetCursorPosition(newPositionX, newPositionY);

                // set new reference
                _lastDragX = touchPoint.Location.X;
                _lastDragY = touchPoint.Location.Y;
            }
        }

        private void Process(CustomDragCompleteData data)
        {
            Debug.WriteLine("InputEmulator: Processing custom drag input completed (button up)");

            // reset and send "mouse button up"
            _lastDragX = null;
            _lastDragY = null;
            _dragMouseButtonDownSend = false;

            Win32Wrapper.SendMouseLeftButtonUp();
        }

        private void Process(TextData data)
        {
            // this simply passes on the transmitted text data as keyboard input to the operating system
            Win32Wrapper.SendKeyboardInput(data.Text);
        }
    }
}