﻿#pragma checksum "C:\Users\Codinpsycho\Desktop\Project D\PAARC_0.9_src\PAARC.WP7\Controls\GrabberControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "279698F50A15C1D143354F39914C811A"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace PAARC.WP7.Controls {
    
    
    public partial class GrabberControl : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.VisualStateGroup OrientationStateGroup;
        
        internal System.Windows.VisualState LandscapeLeft;
        
        internal System.Windows.VisualState LandscapeRight;
        
        internal System.Windows.VisualStateGroup EnabledStateGroup;
        
        internal System.Windows.VisualState Enabled;
        
        internal System.Windows.VisualState Disabled;
        
        internal System.Windows.Controls.Border DragBorder;
        
        internal System.Windows.Controls.Grid grid;
        
        internal System.Windows.Controls.TextBlock GrabberTextBlock;
        
        internal System.Windows.Controls.TextBlock ExplanationTextBlock;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/PAARC.WP7;component/Controls/GrabberControl.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.OrientationStateGroup = ((System.Windows.VisualStateGroup)(this.FindName("OrientationStateGroup")));
            this.LandscapeLeft = ((System.Windows.VisualState)(this.FindName("LandscapeLeft")));
            this.LandscapeRight = ((System.Windows.VisualState)(this.FindName("LandscapeRight")));
            this.EnabledStateGroup = ((System.Windows.VisualStateGroup)(this.FindName("EnabledStateGroup")));
            this.Enabled = ((System.Windows.VisualState)(this.FindName("Enabled")));
            this.Disabled = ((System.Windows.VisualState)(this.FindName("Disabled")));
            this.DragBorder = ((System.Windows.Controls.Border)(this.FindName("DragBorder")));
            this.grid = ((System.Windows.Controls.Grid)(this.FindName("grid")));
            this.GrabberTextBlock = ((System.Windows.Controls.TextBlock)(this.FindName("GrabberTextBlock")));
            this.ExplanationTextBlock = ((System.Windows.Controls.TextBlock)(this.FindName("ExplanationTextBlock")));
        }
    }
}
