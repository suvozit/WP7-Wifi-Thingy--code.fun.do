   M S B A M L   `   `   `  ���� G  CPCController, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:PCController.ViewModelsPCController.ViewModels  *    $PCController.Views.VersionInfoWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     IPCController_ViewModels%clr-namespace:PCController.ViewModels     Hi:http://schemas.microsoft.com/expression/2010/interactivity     Hei9http://schemas.microsoft.com/expression/2010/interactions       � Title$!  Windows Phone PC Controller��$	��400��$	��500�� � ShowInTaskbar .   � WindowStyle$ 
ToolWindow=� � WindowStartupLocation$ CenterOwner=� � 	Resources    VersionDataTemplate � RowDefinitions �� Orientation �� StringFormat  ChangeDataTemplate � ColumnDefinitions	 �� Mode��%�  &	        &	 "    �� .R�� $��0,0,0,5q� �� $
��Auto ��� $
��Auto �.+��� $ 
Horizontal=�.+��� $
��Wrap=�$
��Bold"�$��0,0,-0.001,0q����� *VersionNumber+�� $
��Wrap=�$��10,0,0,0q����� *ReleaseDate+$ ({0:D})���� $��1��$��10,0,0,0q�#���  ���� *	Changes+�� .R��  �� $
��Auto ��� .+�A� $��0�����	#FFF4F4F5$
��Left=�
���   �$��5��$��5��$	��Top=�$��0,6,5,0q��� $
��Wrap=�$��1������ $	 OneWay=�.��� $��5q� �� $
��Auto ��� .+��� $��0q�$ Vertical=�$	��Top=�.+��� $
��Left=�$
��Wrap=��� $A�;A new version of the application is available for download.���� $ 
Horizontal=�.+��� $
��Wrap=��� $�Your current version is:���� $
��Wrap=�$��10,0,0,0q�
���  ������ *CurrentVersion+$	 OneWay=��� $ 
Horizontal=�.+��� $
��Wrap=��� $&� The newest available version is:���� $
��Wrap=�$��10,0,0,0q�
���  ������ *NewestVersion+$	 OneWay=��� $��0,10,0,0q�$��Center=��� -   ���� *DownloadUri+.��� $�Download the installer���� $
��Wrap=�$��0,10,0,0q��� $'�!Version history and new features:���� $��0q�$��1��#���   $��Disabled=����� *
Versions+$	 OneWay=�