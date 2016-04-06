using MurkysRustBoot.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MurkysRustBoot
{
    /// <summary>
    /// Interaction logic for ItemPicker.xaml
    /// </summary>
    public partial class ItemPicker : Window
    {

        public ObservableCollection<RustItem> PickedRustItems
        {
            get; set;
        }

        public RustItem PickedItem;

        public List<RustItem> RustItemsToGive;

        public ItemPicker()
        {
            InitializeComponent();
            InitCategories();
            InitCollections();
            DisableControls();
            DisableAddRemove();
        }

        private void DisableControls()
        {
            btn_Give.IsEnabled = false;
        }

        private void EnableAddRemove()
        {
            EnableAddBtns();
            btn_Remove_Item.IsEnabled = true;
        }

        private void DisableAddRemove()
        {
            DisableAddBtns();
            btn_Remove_Item.IsEnabled = false;
        }

        private void EnableAddBtns()
        {
            btn_Add_Item.IsEnabled = true;
            btn_Add_BP.IsEnabled = true;
        }

        private void DisableAddBtns()
        {
            btn_Add_Item.IsEnabled = false;
            btn_Add_BP.IsEnabled = false;
        }
        
        private void InitCollections()
        {
            PickedRustItems = new ObservableCollection<RustItem>();
            list_Picked_Items.ItemsSource = PickedRustItems;
            ((INotifyCollectionChanged)list_Picked_Items.Items).CollectionChanged +=
        list_Picked_Items_CollectionChanged;
        }

        private void list_Picked_Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                btn_Give.IsEnabled = true;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (list_Picked_Items.Items.Count == 0)
                {
                    btn_Give.IsEnabled = false;
                }
            }
        }

        private void InitCategories()
        {
            foreach (var category in RustItems.Categories()) // List<string>
            {
                var tab = new TabItem();
                var list = new ListBox();
                tab.Header = category;

                list.Background = MainWindow.GetColorBrush("BorderDarkGreen");
                list.Foreground = new SolidColorBrush(Colors.White);
                list.BorderThickness = new Thickness(0, 0, 0, 0);
                list.ItemTemplate = FindResource("TemplatePickedRustItem") as DataTemplate;
                list.SelectionChanged += RustItemListChanged;
                list.SelectionMode = SelectionMode.Extended;
                tab.Content = list;
                CategoryTabControl.Items.Add(tab);
            }
            foreach (var rustItem in RustItems.ToList())
            {
                if (rustItem.IsAllowed)
                {
                    foreach (TabItem category in CategoryTabControl.Items)
                    {
                        if (rustItem.Category == category.Header.ToString())
                        {
                            (category.Content as ListBox).Items.Add(rustItem);
                        }
                    }
                }
            }

        }

        List<RustItem> SelectedRustItemsFromCategory;

        private void RustItemListChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedIndex != -1)
            {
                SelectedRustItemsFromCategory = (sender as ListBox).SelectedItems.Cast<RustItem>().ToList();
                EnableAddBtns();
            }
        }
        
        int _rustItemAmount;

        private void txt_Amount_KeyUp(object sender, KeyEventArgs e)
        {
            slider_Amount.Value = (sender as TextBox).Text.ToDouble();
            _rustItemAmount = slider_Amount.Value.ToInt();
            IsItemAmountInRange();
        }
        
        private void slider_Amount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            txt_Amount.Text = e.NewValue.ToString();
            _rustItemAmount = e.NewValue.ToInt();
            IsItemAmountInRange();
        }

        private void IsItemAmountInRange()
        {
            if (_rustItemAmount > 0)
            {
                EnableAddBtns();
            }
            else
            {
                DisableAddBtns();
            }
        }

        private void btn_Add_Item_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRustItemsFromCategory != null && SelectedRustItemsFromCategory.Count > 0 && _rustItemAmount > 0)
            {
                SelectedRustItemsFromCategory.ForEach(x => {
                    x.Amount = _rustItemAmount;
                    PickedRustItems.Add(x);
                });
            }
        }

        private void btn_Add_BP_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRustItemsFromCategory != null && SelectedRustItemsFromCategory.Count > 0)
            {
                SelectedRustItemsFromCategory.ForEach(x => {
                    x.IsBP = true;
                    x.DisplayName = "(BP) " + x.DisplayName;
                    x.Amount = 0;
                    PickedRustItems.Add(x);
                });
            }
        }

        private void btn_Remove_Item_Click(object sender, RoutedEventArgs e)
        {
            var pickedRustItems = list_Picked_Items.SelectedItems.Cast<RustItem>().ToList();
            pickedRustItems.ForEach(x => {
                PickedRustItems.Remove(x);
            });
        }
        
        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+");
            return !regex.IsMatch(text);
        }

        private void txt_Amount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void list_Picked_Items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedIndex != -1 && (sender as ListBox).SelectedItems.Count > 0)
            {
                btn_Remove_Item.IsEnabled = true;
            }
            else
            {
                btn_Remove_Item.IsEnabled = false;
            }
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btn_Give_Click(object sender, RoutedEventArgs e)
        {
            RustItemsToGive = list_Picked_Items.Items.Cast<RustItem>().ToList();
            Console.WriteLine(RustItemsToGive[0].DisplayName);
            DialogResult = true;
        }

        private void window_ItemPicker_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }


    }
}
public static class ExtensionMethods
{

    public static double ToDouble(this string value)
    {
        double _double;
        double.TryParse(value.ToString(), out _double);
        return _double;
    }

    public static int ToInt(this double value)
    {
        int _int;
        Int32.TryParse(value.ToString(), out _int);
        return _int;
    }
    public static string Sanitize(this string value)
    {
        value = value.ToLower().Trim().Replace(" ", "_");
        return Regex.Replace(value, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
    }
}