using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSL
{
    public class MockDataStore : IDataStore<PickupItem>
    {
        List<PickupItem> items;

        public MockDataStore()
        {
            items = new List<PickupItem>();
            var mockItems = new List<PickupItem>
            {
                new PickupItem { Id = Guid.NewGuid().ToString(), Text = "First item", Location = "Bellevue", Description="This is an item description." },
                new PickupItem { Id = Guid.NewGuid().ToString(), Text = "Second item",Location = "Seattle", Description="This is an item description." },
                new PickupItem { Id = Guid.NewGuid().ToString(), Text = "Third item", Location = "Auburn", Description="This is an item description." },
                new PickupItem { Id = Guid.NewGuid().ToString(), Text = "Fourth item", Location = "Tacoma", Description="This is an item description." },
                new PickupItem { Id = Guid.NewGuid().ToString(), Text = "Fifth item", Location = "Bellevue", Description="This is an item description." },
                new PickupItem { Id = Guid.NewGuid().ToString(), Text = "Sixth item", Location = "Seattle", Description="This is an item description." },
            };

            foreach (var item in mockItems)
            {
                items.Add(item);
            }
        }

        public async Task<bool> AddPickupItemAsync(PickupItem item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdatePickupItemAsync(PickupItem item)
        {
            var _item = items.Where((PickupItem arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(_item);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeletePickupItemAsync(string id)
        {
            var _item = items.Where((PickupItem arg) => arg.Id == id).FirstOrDefault();
            items.Remove(_item);

            return await Task.FromResult(true);
        }

        public async Task<PickupItem> GetPickupItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<PickupItem>> GetPickupItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}
