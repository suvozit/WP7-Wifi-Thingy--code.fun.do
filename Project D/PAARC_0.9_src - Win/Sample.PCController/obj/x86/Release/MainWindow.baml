   M S B A M L   `   `   `  ���� G  CPCController, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:PCController.ViewModelsPCController.ViewModels  A%clr-namespace:PCController.ConvertersPCController.Converters      PCController.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     <
viewModels%clr-namespace:PCController.ViewModels     <
converters%clr-namespace:PCController.Converters     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -     � Title$  WP7 Remote Controller�� � WindowStyle$
 None=� � 
ResizeMode$ CanMinimize=�$	��500��$	��640�� � Icon$ App.ico�����	#FF1BA1E2
������� � 	Resources 6   0PCController.Converters.BooleanAndMultiConverter   AndConverter  ButtonControlTemplate �� 
TargetType'  !System.Windows.VisualStateManager   VisualStateGroups%  System.Windows.VisualStateGroup   States   System.Windows.VisualState   
Storyboard	 �� TargetProperty
 �� 
TargetName9  3System.Windows.Media.Animation.EasingDoubleKeyFrame O� KeyTime O� Value8  2System.Windows.Media.Animation.EasingColorKeyFrame �� KeyTime �� Value ��Name �� Padding J� Opacity ��Name  TextBlockCaptionStyle �� 
TargetType �� Property �� Value��%  &	        &	      &	 R      �� " ��.R��    CommonStates��.   
Normal��  	MouseOver��. �� .��U� $	 (UIElement.Opacity)�$
 HighlightBorder��.\�  $ 0��$ 1Q�  Pressed��. �� .��U� $	 (UIElement.Opacity)�$
 HighlightBorder��.\�  $ 0��$ 1Q��� $0	 *(Panel.Background).(SolidColorBrush.Color)�$
 HighlightBorder��.g�  $ 0��$ 	#FFFFB4B4��  Disabled��.+��� $ HighlightBorder��$��0q�$��Stretch=�$��Stretch=�#��z��$��1q�$ 0q�$ 0Q����	#00FFB4B4�� $ 	textBlock��$
��Wrap=�#��z��#��z��$��Right=�#��z����Z� �� " ��.���  � ��$ NoWrap=���  � ��$
 None=���  � ��$ Segoe UI Light)���  � ��$ 48(���  � ��$ 0,0,0,-8q���  � ��$ Right=� � DataContext 1   +PCController.ViewModels.MainWindowViewModel  .���  � ColumnDefinitions �� �� $
��Auto � � RowDefinitions �� $
��Auto ��� .+��� $��1��$��Bottom=�$
��Left=�$��10(�$��5 0 0 3q����� *ApplicationVersion+��  �� Orientation$ 
Horizontal=�.+��� $��62��$��0q�$ ��Content/Images/Tile_62.png��$
��None=�$	��Top=�$
��Left=� J� IsHitTestVisible .  �� $
��Left=�$
��Wrap=�$!��Windows Phone PC Controller��$��Bottom=�$��0,0,0,3q�$��Segoe WP Light)�$��26.667(� .  �� $��Right=�$��0,3,3,0q�$��1��$ 
Horizontal=�$	��Top=�.+��� -   $��_��#���  $��0,2,2,0q�
�������$��3,-5,3,3q� � ToolTip$ Minimize to System Tray���� -   $��x��#���  $��0,2,2,0q�
�������$��3,-3,3,1q�$	��Top=�$ Shutdown and Close��� $��	0,15,0,20q�$��1��$��2�� �� $
��Auto ��� $
��Auto ��� �� $
��Auto � �� $	��200 ��� .+��� $
��Wrap=�$
��info��$��Segoe UI Light)�$��48(�#���  �� $
��20,0q�$��Bottom=�$��1��.+��� $��0,0,0,2q�$ 
Horizontal=�.+��� $
��Wrap=�$��Status:���� $
��Wrap=�$	��5,0q����� *Status+�� $ 
Horizontal=�$��0q�.+��� $#��IP address of this computer: ��$��0q��� $
��Wrap=�$	��5,0q����� *	IPAddress+�� $
��Wrap=�$��options��#���  $��0q�$��1���� $��
19,26,20,0q�$��1��$��1��.+��� $��Use accelerometer��$
��Left=�  CheckBoxStyle#���  
������� d� 	IsChecked �� * Configuration.UseAccelerometer+ �� Mode$ TwoWay=�{��� *IsPhoneDeviceReady+�� -    ��Name$ UseTouchCheckBox��$��Use touch input��$
��Left=�
������� �� *Configuration.UseTouchInput+$ TwoWay=�{��� *IsPhoneDeviceReady+�� $��Use relative positioning��$
��Left=�$��18,0,0,0q�
������� �� *$"Configuration.TouchInputIsRelative+$ TwoWay=�{�`�  `� 	Converter# [  .1���   �� Path$  IsPhoneDeviceReady��� $  	IsChecked�! �� ElementName$! UseTouchCheckBox���� $��Use tap gestures��$
��Left=�
������� �� *Configuration.UseTapGestures+$ TwoWay=�{��� *IsPhoneDeviceReady+�� $��Use double-tap gestures��$
��Left=�
������� �� *$"Configuration.UseDoubleTapGestures+$ TwoWay=�{��� *IsPhoneDeviceReady+�� $��Use hold gestures��$
��Left=�
������� �� *Configuration.UseHoldGestures+$ TwoWay=�{��� *IsPhoneDeviceReady+�� $��Use flick gestures��
������� �� * Configuration.UseFlickGestures+$ TwoWay=�{��� *IsPhoneDeviceReady+�� $��Use drag gestures��$
��Left=�
������� �� *Configuration.UseDragGestures+$ TwoWay=�{��� *IsPhoneDeviceReady+�� $��Use text input��$
��Left=�
������� �� *Configuration.UseTextInput+$ TwoWay=�{��� *IsPhoneDeviceReady+�� -   $ EnableTracingCheckBox��$��Enable Tracing��$y�	Collapsed=�$
��Left=�
������� �� *Configuration.EnableTracing+$ TwoWay=��� -   " ��Name$#" TracingEndpointAddressTextBox��$y�	Collapsed=�$��18,2,0,0q����� *&$Configuration.TracingEndpointAddress+$ TwoWay=��� $
��Wrap=�$	��log��#���  $��0q�$��2���� 
����������	#FFE2E2E2$��
20,27,20,0q�$��2��$��1������ *Errors+�� $��Right=�$
��Wrap=�$	��Top=�#���  $��3��$��	0,0,0,-11q��� $
�help���� $��3��$��1��$
��20,0q�$��Bottom=�
�������$
��Left=�# �� Padding$
# 10,5q��� -   $"��http://www.pitorque.de/paarc?�.��� $"�http://www.pitorque.de/paarc��