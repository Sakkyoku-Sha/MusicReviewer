   M S B A M L   `   `   `  ����H  DMusicReviewer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:MusicReviewerAppMusicReviewerApp  5   	   !    MusicReviewerApp.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:MusicReviewerApp     5   	   -   5          �Name$  MainWindow1��5   	    � Title$ 
MainWindow��5   	   $	��500��6   $	��900��6)    � 
ResizeMode$ NoResize=�5	   	   .��5
      �  � ColumnDefinitions 5   
   �� 5      $
��115* �6   �� 5      $
��623* �6   .+�5   
   ��  �� Orientation$ Vertical=�6   $��90��6,   $
��Left=�67   $��Center=�6S   $��0,4,0,5q�6n   .+�5      �� -   5        $��Review��5      $��90��6&   $��45��6I   �� 5      -   5        $
��Sort��5      $��90��6$   $��45��6I   �� 5      -   5        $
��Tags��5      $��90��6$   $��45��6E   �� 5      -   5        $��Stats��5      $��90��6%   $��45��6G   �� 5      -   5        $
��Quit��5      $��90��6$   $��12��6F   $��Right=�6R   $��92��6n   5       � 5   
   -   5          �Name$
 Main��5      $��90,0,0,0q�6     � NavigationUIVisibility$ Hidden=�6Q   $��2��6q   5      5      