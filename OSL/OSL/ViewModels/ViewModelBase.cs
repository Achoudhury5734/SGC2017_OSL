using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

namespace OSL
{
    public class ViewModelBase : BaseViewModel
    {
        public IDataStore<PickupItem> DataStore => DependencyService.Get<IDataStore<PickupItem>>() ?? new MockDataStore();

    }
}
