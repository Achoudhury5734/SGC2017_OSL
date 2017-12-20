using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;
using OSL.Models;

namespace OSL
{
    public class ViewModelBase : BaseViewModel
    {
        public IDataStore<Donation> DataStore => DependencyService.Get<IDataStore<Donation>>() ?? new MockDataStore();

    }
}
