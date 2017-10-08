using System;

namespace OSL
{
    public class PickupItemDetailViewModel : ViewModelBase
    {
        public PickupItem Item { get; set; }
        public PickupItemDetailViewModel(PickupItem item = null)
        {
            Title = item?.Text;
            Item = item;
        }
    }
}
