   M S B A M L   `   `   `  ����G  CPCController, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:PCController.ViewModelsPCController.ViewModels  5   	   A%clr-namespace:PCController.ConvertersPCController.Converters  5   	       PCController.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   <
viewModels%clr-namespace:PCController.ViewModels     5   	   <
converters%clr-namespace:PCController.Converters     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -   5          � Title$  WP7 Remote Controller��5	   	    � WindowStyle$
 None=�5
   	    � 
ResizeMode$ CanMinimize=�5   	   $	��500��5   	   $	��640��5   	    � Icon$ App.ico��5   	   ���	#FF1BA1E2
������� � 	Resources 5      6   0PCController.Converters.BooleanAndMultiConverter   AndConverter  ButtonControlTemplate �� 
TargetType'  !System.Windows.VisualStateManager   VisualStateGroups%  System.Windows.VisualStateGroup   States   System.Windows.VisualState   
Storyboard	 �� TargetProperty
 �� 
TargetName9  3System.Windows.Media.Animation.EasingDoubleKeyFrame O� KeyTime O� Value8  2System.Windows.Media.Animation.EasingColorKeyFrame �� KeyTime �� Value ��Name �� Padding J� Opacity ��Name  TextBlockCaptionStyle �� 
TargetType �� Property �� Value��%�  &	        &	      &	 K      5   
   6.   �� 5   
   " ��5      .R�5      �  5        5      CommonStates��. 5        
Normal��6&     5      	MouseOver��. 5      �� .��5   "   U� $	 (UIElement.Opacity)�6@   $
 HighlightBorder��5    @   .\�5!   &     $ 0��6;   $ 1Q�5"   ;   5#   #   5$      5%        5&      Pressed��. 5'      �� .��5(   "   U� $	 (UIElement.Opacity)�6@   $
 HighlightBorder��5)   @   .\�5*   &     $ 0��6;   $ 1Q�5+   ;   5,   #   �� 5-   "   $0	 *(Panel.Background).(SolidColorBrush.Color)�6?   $
 HighlightBorder��5.   ?   .g�5/   &     $ 0��6:   $ 	#FFFFB4B4��50   :   51   #   52      53        54      Disabled��6&   55      .+�57      �� $ HighlightBorder��6   $��0q�58      $��Stretch=�5:      $��Stretch=�5;      #��z��5<      $��1q�5=      $ 0q�5>      $ 0Q�5?      ���	#00FFB4B45@      �� 5A      $ 	textBlock��6   $
��Wrap=�5B      #��z��5C      #��z��5D      $��Right=�5E      #��z��5F      ��5H      Z� 5I      5J      �� 5K   
   " ��5L      .�5M      ��  � ��$ NoWrap=�5N      �� 5O       � ��$
 None=�5P      �� 5Q       � ��$ Segoe UI Light)�5R      �� 5S       � ��$ 48(�5T      �� 5U       � ��$ 0,0,0,-8q�5V      �� 5W       � ��$ Right=�5X      5Y      5Z       � DataContext 5[      1   +PCController.ViewModels.MainWindowViewModel  5\   
   .��5^      �  � ColumnDefinitions 5_   
   �� 5`      �� 5a      $
��Auto �6    � RowDefinitions 5c   
   �� 5d      $
��Auto �6   �� 5e      .+�5h   
   �� $��1��5i      $��Bottom=�5j      $
��Left=�5k      $��10(�5l      $��5 0 0 3q�5m      ��5h      �� *ApplicationVersion+5m      �� 5o   
    �� Orientation$ 
Horizontal=�6   .+�5p      �� $��62��6   $��0q�5q      $ ��Content/Images/Tile_62.png��5r      $
��None=�5s      $	��Top=�5t      $
��Left=�5u       J� IsHitTestVisible .  5v      �� 5w      $
��Left=�6   $
��Wrap=�5x      $!��Windows Phone PC Controller��5y      $��Bottom=�5z      $��0,0,0,3q�5{      $��Segoe WP Light)�5|      $��26.667(�5}       .  5~      5      �� 5�   
   $��Right=�6   $��0,3,3,0q�5�      $��1��5�      $ 
Horizontal=�5�      $	��Top=�5�      .+�5�      �� -   5        $��_��5�      #���  5�      $��0,2,2,0q�5�      
�������$��3,-5,3,3q�5�       � ToolTip$ Minimize to System Tray��5�      �� 5�      -   5        $��x��5�      #���  5�      $��0,2,2,0q�5�      
�������$��3,-3,3,1q�5�      $	��Top=�5�      $ Shutdown and Close��5�      5�      � 5�   
   $��	0,15,0,20q�6   $��1��5�      $��2��5�       5�      �� 5�      $
��Auto �6    �� 5�      $
��Auto �6    �� 5�      �� 5�      $
��Auto �6     5�      �� 5�      $	��200 �6#   �� 5�      .+�5�      �� $
��Wrap=�6   $
��info��5�      $��Segoe UI Light)�5�      $��48(�5�      #���  5�      �� 5�      $
��20,0q�6   $��Bottom=�5�      $��1��5�      .+�5�      �� $��0,0,0,2q�6   $ 
Horizontal=�5�      .+�5�      �� $
��Wrap=�6    $��Status:��5�       �� 5�      $
��Wrap=�6    $	��5,0q�5�       ��5�       �� *Status+5�       5�      �� 5�      $ 
Horizontal=�6   $��0q�5�      .+�5�      �� $#��IP address of this computer: ��6    $��0q�5�       �� 5�      $
��Wrap=�6    $	��5,0q�5�       ��5�       �� *	IPAddress+5�       5�      5�      �� 5�      $
��Wrap=�6   $��options��5�      #���  5�      $��0q�5�      $��1��5�      �� 5�      $��
19,26,20,0q�6   $��1��5�      $��1��5�      .+�5�      �� $��Use accelerometer��6   $
��Left=�5�        CheckBoxStyle#���  5�      
������� d� 	IsChecked 5�      �� * Configuration.UseAccelerometer+ �� Mode$ TwoWay=�{�5�      �� *IsPhoneDeviceReady+5�      �� 5�      -   5         ��Name$ UseTouchCheckBox��5�      $��Use touch input��5�      $
��Left=�5�      
������� 5�      �� *Configuration.UseTouchInput+$ TwoWay=�{�5�      �� *IsPhoneDeviceReady+5�      �� 5�      $��Use relative positioning��6   $
��Left=�5�      $��18,0,0,0q�5�      
������� 5�      �� *$"Configuration.TouchInputIsRelative+$ TwoWay=�{�5�      `� 5�       `� 	Converter# [  6'   .1�5�      ��   �� Path$  IsPhoneDeviceReady�6&   �� 5�      $  	IsChecked�6&   ! �� ElementName$! UseTouchCheckBox��5�   &   5�      5�      �� 5�      $��Use tap gestures��6   $
��Left=�5�      
������� 5�      �� *Configuration.UseTapGestures+$ TwoWay=�{�5�      �� *IsPhoneDeviceReady+5�      �� 5�      $��Use double-tap gestures��6   $
��Left=�5�      
������� 5�      �� *$"Configuration.UseDoubleTapGestures+$ TwoWay=�{�5�      �� *IsPhoneDeviceReady+5�      �� 5�      $��Use hold gestures��6   $
��Left=�5�      
������� 5�      �� *Configuration.UseHoldGestures+$ TwoWay=�{�5�      �� *IsPhoneDeviceReady+5�      �� 5�      $��Use flick gestures��6   
������� 5�      �� * Configuration.UseFlickGestures+$ TwoWay=�{�5�      �� *IsPhoneDeviceReady+5�      �� 5�      $��Use drag gestures��6   $
��Left=�5�      
������� 5�      �� *Configuration.UseDragGestures+$ TwoWay=�{�5�      �� *IsPhoneDeviceReady+5�      �� 5�      $��Use text input��6   $
��Left=�5�      
������� 5�      �� *Configuration.UseTextInput+$ TwoWay=�{�5�      �� *IsPhoneDeviceReady+5�      �� 5�      -   5        $ EnableTracingCheckBox��5�      $��Enable Tracing��5�      $y�	Collapsed=�5�      $
��Left=�5      
������� 5�      �� *Configuration.EnableTracing+$ TwoWay=�5     �� 5     -   5        " ��Name$#" TracingEndpointAddressTextBox��5     $y�	Collapsed=�5     $��18,2,0,0q�5     ��5     �� *&$Configuration.TracingEndpointAddress+$ TwoWay=�5     5     �� 5     $
��Wrap=�6   $	��log��5	     #���  5
     $��0q�5     $��2��5     �� 5     
����������	#FFE2E2E2$��
20,27,20,0q�5     $��2��5     $��1��5     ��5     �� *Errors+5     �� 5     $��Right=�6   $
��Wrap=�5     $	��Top=�5     #���  5     $��3��5     $��	0,0,0,-11q�5     �� 6,   $
�help��60   6@   �� 5     $��3��6   $��1��5     $
��20,0q�5     $��Bottom=�5     
�������$
��Left=�5     # �� Padding$
# 10,5q�5      �� 5!     -   5        $"��http://www.pitorque.de/paarc?�5!     .�5"  I   �� $"�http://www.pitorque.de/paarc��6M   6u   5#     5$     5%     5&     