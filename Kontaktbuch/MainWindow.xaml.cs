using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Kontaktbuch
{
    public partial class MainWindow : Window
    {
        private List<PersonModel> people = new();
        private int selectedID = -1;
        private PersonModel currentSelectedPerson;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            Leeren();
            personGridView.Columns[0].Width = 0;
            PersonListView_SizeChanged(personListView, null);
            LoadPeopleList();
        }

        private void LoadPeopleList()
        {
            people = SqliteDataAccess.LoadPeople();
            personListView.ItemsSource = people;
            Leeren();
        }

        private void addPersonButton_Click(object sender, EventArgs e)
        {
            PersonAddWindow personAddWindow = new();
            personAddWindow.ShowDialog();

            LoadPeopleList();
        }

        private void PersonListView_SelectionChanged(object sender, EventArgs e)
        {
            foreach (PersonModel p in personListView.SelectedItems)
            {
                selectedID = p.Id;
                currentSelectedPerson = p;
            }
        }

        private void Leeren()
        {
            personListView.SelectedItem = null;

            personListView.UnselectAll();
            selectedID = -1;
            currentSelectedPerson = null;
        }

        private void PersonLöschen_Click(object sender, EventArgs e)
        {
            if (currentSelectedPerson != null)
            {
                if (MessageBox.Show("Möchten sie die Einträge wirklich löschen?", "Bestätigung", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (personListView != null)
                    {
                        foreach (PersonModel p in personListView.SelectedItems)
                        {
                            SqliteDataAccess.DeletePerson(p);
                        }
                    }
                    LoadPeopleList();
                }
            }
        }

        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(txtFilter.Text))
            {
                return true;
            }
            else
            {
                return (item as PersonModel).VorName.Contains(txtFilter.Text, StringComparison.OrdinalIgnoreCase);
            }
        }

        private void TxtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(personListView.ItemsSource);
            view.Filter = UserFilter;
        }

        private void PersonListView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (r.VisualHit.GetType() == typeof(ScrollViewer))
            {
                Leeren();
            }
        }

        private void PersonListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gridView = listView.View as GridView;

            double width = (listView.ActualWidth - SystemParameters.VerticalScrollBarWidth - gridView.Columns[0].Width) / 8;

            foreach (GridViewColumn gridViewColumn in personGridView.Columns)
            {
                if (gridViewColumn.Header.ToString() != "Id")
                {
                    gridViewColumn.Width = width;
                }
            }
        }

        private void PersonListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (r.VisualHit.GetType() != typeof(ScrollViewer))
            {
                if(currentSelectedPerson != null)
                {
                    PersonAddWindow personAddWindow = new PersonAddWindow(currentSelectedPerson);
                    personAddWindow.ShowDialog();
                    LoadPeopleList();
                }
            }
        }
    }
}
