   M S B A M L   `   `   `  ����I  EMurkysRustBoot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:MurkysRustBootMurkysRustBoot  5   	       MurkysRustBoot.ItemPicker   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:MurkysRustBoot     5   	   -   5          �Name$  window_ItemPicker��5   	    � Title$ 
ItemPicker��5   	   $	��550��6   $	��550��6)    � WindowStartupLocation$ CenterOwner=�65    � 
ResizeMode$ NoResize=�6Y    � AllowsTransparency .  � WindowStyle$
 None=�6�   .��5	      � ��5
   
   �� 5       �� ImageSource$ Images/bg_Rust.png��6    � RowDefinitions 5   
   �� 5      $��40 �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $	��140 �6   �� 5      .+�5   
   �� $��0��6   $��0��6"   $��2��62   	 �� Orientation$	 
Horizontal=�6F      BorderDarkGreen#��[  6_   .+�5      �� $��Center=�6   $��5q�63   ���#fbebe0$
��Bold"�6S   $��14(�6e   $��Items��6s   5      �� 5   
   $��0��6   $��1��6"   $	 
Horizontal=�62   $��Right=�6K   $��20��6h   .+�5      �� -   5        
 ��Name$
 	btn_Close��5      $��14(�6(   $
��Bold"�66     Btn_Template#��[ 6H     	DangerRed#��[ 6n   $��20��6�   $��20��6�   .��6�   X5      5      ]� 5   
   $��1��6   $��1��6!   .+�5      ]� $��Bottom=�6   $��30��60   $��0,0,0,8q�6<   .+�5      �� $��Right=�6   $	 
Horizontal=�64   .+�5       �� -   5         ��Name$ 
txt_Amount��5       $
��Left=�62   $��40��6I   $	��8,0q�6T   $��0��6a    �� TextAlignment$ Center=�6j   $��16(�6�   $��0,4,0,0q�6�   #��[  6�   
�������6�   �� 5"      -   5        $
 btn_Give��5"      #��[ 6/     SuccessGreen#��[ 6U     SuccessGreen_Highlighted#��[ 6�   $
��Give��6�   $��30��6�   $��60��6�   6�   �� 5$      -   5        $
 
btn_Cancel��5$      #��[ 61   #��[ 6W     DangerRed_Highlighted#��[ 6   $��Close��6�   $��30��6�   $��60��6�   6�   5%      �� 5&      -   5         ��Name$ slider_Amount��5&      $
��Left=�60   $��8,0,0,0q�6F   $��Center=�6W    � Maximum$
 1000Q�6r    � SmallChange$ 1Q�6�    � LargeChange$ 20Q�6�    � Cursor$
 Hand��6�    �� IsSnapToTickEnabled . 6�   5'      �� 5(      -   5         ��Name$ CategoryTabControl��5(      
������ $��0q�6e   ��6N   Z�  � 	Resources 5)       �� 
TargetType �� Property �� Value  	tabs_Item �� Triggers P� Property P� Value  InfoBlue_Highlighted��%�  '��       '�� L     7  7  7  �� 5*      " ��6   .�5+      ��  � ��$ Center=�6@   5,      �� 5-      " ��6   .�5.      ��  � ��# �  65    5/      P� 50       �@��
IsSelected . . �51   "   ��  � ��8   6o   52      P� 53       �@J�IsMouseOver . . �54   "   ��  � ��8  6o   55      P� 56       �@��
IsSelected .  . �57   "   ��  � ��8  6o   58      5:      5;      5<      5=      ]� 5>   
   $��1��6   $��0��6!   .+�5?      � $��Bottom=�6   $��8q�6+   $��30��66    5@      �� 5A      $
��.53* �6'   �� 5B      $
��.36* �6'   �� 5C      $��* �6'   .+�5E      �� -   5        $
 btn_Add_Item��5E      $��0,0,4,0q�6/   $��30��6@   $��12(�6L   $��0��6Z   #��[ 6j   #��[ 6�   #��[ 6�   $	��Add��6�   6   �� 5F      -	   5        $
 
btn_Add_BP��5F      #��[ 6-   $��12(�6S   $��1��6a   $��0q�6q     InfoBlue#��[ 6|   #��[ 6�   $��BP��6�   $��30��6�   6�   �� 5G      -
   5        $
 btn_Remove_Item��5G      $��4,0,0,0q�62   $��30��6C   $��12(�6O   $��2��6]   #��[ 6m   #��[ 6�   #��[ 6�   $��Remove��6�   6   5H      �� 5I      -   5         ��Name$ list_Picked_Items��5I      $	��Top=�61   $��Stretch=�6F   #��[  6l   
������� �� SelectionMode$ Extended=�6�    	 TemplatePickedRustItemsList#��[	 6�   $��0q�6O  ��68  Z� 6O  5J      5K      5L      