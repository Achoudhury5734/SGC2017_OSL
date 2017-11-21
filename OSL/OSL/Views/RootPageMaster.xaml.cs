using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OSL.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RootPageMaster : ContentPage
    {
        public ListView ListView;

        public RootPageMaster()
        {
            InitializeComponent();

            BindingContext = new RootPageMasterViewModel();
            ListView = MenuItemsListView;
        }

        class RootPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<RootPageMenuItem> MenuItems { get; set; }

            public RootPageMasterViewModel()
            {
                if (App.User.Recipient)
                {
                    MenuItems = new ObservableCollection<RootPageMenuItem>(new[]
                    {
                        new RootPageMenuItem { Id = 0, Title = "Available", TargetType=typeof(PickupItemsPage) },
                        new RootPageMenuItem { Id = 5, Title = "Accepted", TargetType=typeof(AcceptedTabPage)},
                        new RootPageMenuItem { Id = 1, Title = "Pending", TargetType=typeof(AcceptedItemsPage)},
                        new RootPageMenuItem { Id = 2, Title = "Completed", TargetType=typeof(AcceptedItemsPage)},
                        new RootPageMenuItem { Id = 3, Title = "Logout", TargetType=typeof(MainPage) },
                        new RootPageMenuItem { Id = 4, Title = ""},
                        new RootPageMenuItem { Id = 6, Title = ""},
                        new RootPageMenuItem { Id = 7, Title = "About", TargetType=typeof(AboutPage) }
                    });
                }
                else
                {
                    MenuItems = new ObservableCollection<RootPageMenuItem>(new[]
                    {
                        new RootPageMenuItem { Id = 0, Title = "Donate", TargetType=typeof(DonationPage) },
                        new RootPageMenuItem { Id = 8, Title = "My Donations", TargetType=typeof(DonationTabPage) },
                        new RootPageMenuItem { Id = 1, Title = "Listed", TargetType=typeof(DonationListPage) },
                        new RootPageMenuItem { Id = 2, Title = "Pending", TargetType=typeof(DonationListPage) },
                        new RootPageMenuItem { Id = 3, Title = "Completed", TargetType=typeof(DonationListPage) },
                        new RootPageMenuItem { Id = 4, Title = "Wasted", TargetType=typeof(DonationListPage) },
                        new RootPageMenuItem { Id = 5, Title = "YTD Tally", TargetType=typeof(YTDTallyPage) },
                        new RootPageMenuItem { Id = 6, Title = "Logout", TargetType=typeof(MainPage) },
                        new RootPageMenuItem { Id = 7, Title = "About", TargetType=typeof(AboutPage) }
                    });
                }
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}
