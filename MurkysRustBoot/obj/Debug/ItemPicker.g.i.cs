﻿#pragma checksum "..\..\ItemPicker.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "BB79061B4D4829754D70116FD6D003C7"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MurkysRustBoot;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace MurkysRustBoot {
    
    
    /// <summary>
    /// ItemPicker
    /// </summary>
    public partial class ItemPicker : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 1 "..\..\ItemPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MurkysRustBoot.ItemPicker window_ItemPicker;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\ItemPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_Close;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\ItemPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt_Amount;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\ItemPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_Give;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\ItemPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_Cancel;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\ItemPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider slider_Amount;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\ItemPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl CategoryTabControl;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\ItemPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_Add_Item;
        
        #line default
        #line hidden
        
        
        #line 70 "..\..\ItemPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_Add_BP;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\ItemPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_Remove_Item;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\ItemPicker.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox list_Picked_Items;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MurkysRustBoot;component/itempicker.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ItemPicker.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.window_ItemPicker = ((MurkysRustBoot.ItemPicker)(target));
            
            #line 8 "..\..\ItemPicker.xaml"
            this.window_ItemPicker.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.window_ItemPicker_MouseDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.btn_Close = ((System.Windows.Controls.Button)(target));
            
            #line 25 "..\..\ItemPicker.xaml"
            this.btn_Close.Click += new System.Windows.RoutedEventHandler(this.btn_Close_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.txt_Amount = ((System.Windows.Controls.TextBox)(target));
            
            #line 32 "..\..\ItemPicker.xaml"
            this.txt_Amount.KeyUp += new System.Windows.Input.KeyEventHandler(this.txt_Amount_KeyUp);
            
            #line default
            #line hidden
            
            #line 32 "..\..\ItemPicker.xaml"
            this.txt_Amount.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.txt_Amount_PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btn_Give = ((System.Windows.Controls.Button)(target));
            
            #line 34 "..\..\ItemPicker.xaml"
            this.btn_Give.Click += new System.Windows.RoutedEventHandler(this.btn_Give_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btn_Cancel = ((System.Windows.Controls.Button)(target));
            
            #line 36 "..\..\ItemPicker.xaml"
            this.btn_Cancel.Click += new System.Windows.RoutedEventHandler(this.btn_Cancel_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.slider_Amount = ((System.Windows.Controls.Slider)(target));
            
            #line 38 "..\..\ItemPicker.xaml"
            this.slider_Amount.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.slider_Amount_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.CategoryTabControl = ((System.Windows.Controls.TabControl)(target));
            return;
            case 8:
            this.btn_Add_Item = ((System.Windows.Controls.Button)(target));
            
            #line 69 "..\..\ItemPicker.xaml"
            this.btn_Add_Item.Click += new System.Windows.RoutedEventHandler(this.btn_Add_Item_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.btn_Add_BP = ((System.Windows.Controls.Button)(target));
            
            #line 70 "..\..\ItemPicker.xaml"
            this.btn_Add_BP.Click += new System.Windows.RoutedEventHandler(this.btn_Add_BP_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.btn_Remove_Item = ((System.Windows.Controls.Button)(target));
            
            #line 71 "..\..\ItemPicker.xaml"
            this.btn_Remove_Item.Click += new System.Windows.RoutedEventHandler(this.btn_Remove_Item_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.list_Picked_Items = ((System.Windows.Controls.ListBox)(target));
            
            #line 73 "..\..\ItemPicker.xaml"
            this.list_Picked_Items.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.list_Picked_Items_SelectionChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
