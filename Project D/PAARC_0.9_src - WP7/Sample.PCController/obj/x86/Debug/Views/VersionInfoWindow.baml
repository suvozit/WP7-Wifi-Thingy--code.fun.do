   M S B A M L   `   `   `  ����G  CPCController, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:PCController.ViewModelsPCController.ViewModels  5   	   *    $PCController.Views.VersionInfoWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   IPCController_ViewModels%clr-namespace:PCController.ViewModels     5   	   Hi:http://schemas.microsoft.com/expression/2010/interactivity     5   	   Hei9http://schemas.microsoft.com/expression/2010/interactions     5   	     � Title$!  Windows Phone PC Controller��5
   	   $	��400��5   	   $	��500��5   	    � ShowInTaskbar .   � WindowStyle$ 
ToolWindow=�5   	    � WindowStartupLocation$ CenterOwner=�5   	    � 	Resources 5         VersionDataTemplate � RowDefinitions �� Orientation �� StringFormat  ChangeDataTemplate � ColumnDefinitions	 �� Mode��%�  &	        &	     �� 5   
   .R�5      � $��0,0,0,5q�6    5      �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   .+�5      �� $ 
Horizontal=�6   .+�5      �� $
��Wrap=�6    $
��Bold"�5       $��0,0,-0.001,0q�5       ��5       �� *VersionNumber+5       �� 5      $
��Wrap=�6    $��10,0,0,0q�5        ��5       �� *ReleaseDate+$ ({0:D})��5        5!      �� 5"      $��1��6   $��10,0,0,0q�5#      #���  5%      ��5$      �� *	Changes+5%      5&      5'      �� 5(   
   .R�5)      �  5*      �� 5+      $
��Auto �6'   �� 5,      .+�5.      A� $��0��6   ���	#FFF4F4F5$
��Left=�50      
���   �$��5��52      $��5��53      $	��Top=�54      $��0,6,5,0q�55      �� 56      $
��Wrap=�6   $��1��59      ��57      �� $	 OneWay=�59      5:      5;      5<      .��5=      � $��5q�6    5>   
   �� 5?      $
��Auto �6   �� 5@      .+�5B   
   �� $��0q�6   $ Vertical=�5C      $	��Top=�5D      .+�5E      �� $
��Left=�6   $
��Wrap=�5F      �� 6-   $A�;A new version of the application is available for download.��61   6x   �� 5G      $ 
Horizontal=�6   .+�5H      �� $
��Wrap=�6   �� 61   $�Your current version is:��65   6Y   �� 5I      $
��Wrap=�6   $��10,0,0,0q�5K      
���  ����5J      �� *CurrentVersion+$	 OneWay=�5L      5M      �� 5N      $ 
Horizontal=�6   .+�5O      �� $
��Wrap=�6   �� 61   $&� The newest available version is:��65   6a   �� 5P      $
��Wrap=�6   $��10,0,0,0q�5R      
���  ����5Q      �� *NewestVersion+$	 OneWay=�5S      5T      �� 5U      $��0,10,0,0q�6   $��Center=�5V      �� 5W      -   5        ��5W      �� *DownloadUri+.�5X   I   �� $�Download the installer��6M   6o   5Y      �� 5Z      $
��Wrap=�6   $��0,10,0,0q�5[      �� 6+   $'�!Version history and new features:��6/   6\   5\      �� 5]   
   $��0q�6   $��1��5^      #���   5`      $��Disabled=�5a      ��5_      �� *
Versions+$	 OneWay=�5a      5b      5c      