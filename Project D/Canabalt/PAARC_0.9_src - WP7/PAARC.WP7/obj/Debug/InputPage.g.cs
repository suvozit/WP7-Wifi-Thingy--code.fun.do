﻿#pragma checksum "C:\Users\Codinpsycho\Desktop\Project D\PAARC_0.9_src\PAARC.WP7\InputPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "CEDC8CBF9D895C14BC809F96FE5DBBC8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using PAARC.WP7.Controls;
using PAARC.WP7.Views;
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


namespace PAARC.WP7 {
    
    
    public partial class InputPage : PAARC.WP7.Views.NavigationPhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal PAARC.WP7.Controls.GrabberControl CustomDragGrabber;
        
        internal PAARC.WP7.Controls.GrabberControl TextInputGrabber;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.TextBlock PageTitle;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/PAARC.WP7;component/InputPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.CustomDragGrabber = ((PAARC.WP7.Controls.GrabberControl)(this.FindName("CustomDragGrabber")));
            this.TextInputGrabber = ((PAARC.WP7.Controls.GrabberControl)(this.FindName("TextInputGrabber")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.PageTitle = ((System.Windows.Controls.TextBlock)(this.FindName("PageTitle")));
        }
    }
}
