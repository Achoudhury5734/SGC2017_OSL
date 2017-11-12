using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OSL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RootPage : MasterDetailPage
    {
        public RootPage()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
            if (App.User.Recipient)
                Detail = new NavigationPage(new PickupItemsPage());
            else
                Detail = new NavigationPage(new DonationPage());
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as RootPageMenuItem;
            if (item == null)
                return;

            if (item.Title == "Logout") {
                Application.Current.MainPage = new MainPage(logout: true);
                return;
            }

            var page = (Page)Activator.CreateInstance(item.TargetType);

            Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }
    }
}