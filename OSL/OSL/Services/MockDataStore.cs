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
                new PickupItem { Id = Guid.NewGuid().ToString(), PictureURL = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/Foods_%28cropped%29.jpg/220px-Foods_%28cropped%29.jpg", Location = "Bellevue", Description="This is an item1 description." },
                new PickupItem { Id = Guid.NewGuid().ToString(), PictureURL = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/Foods_%28cropped%29.jpg/220px-Foods_%28cropped%29.jpg",Location = "Seattle", Description="This is an item2 description." },
                new PickupItem { Id = Guid.NewGuid().ToString(), PictureURL = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/Foods_%28cropped%29.jpg/220px-Foods_%28cropped%29.jpg", Location = "Auburn", Description="This is an item3 description." },
                new PickupItem { Id = Guid.NewGuid().ToString(), PictureURL = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/Foods_%28cropped%29.jpg/220px-Foods_%28cropped%29.jpg", Location = "Tacoma", Description="This is an item4 description." },
                new PickupItem { Id = Guid.NewGuid().ToString(), PictureURL = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/Foods_%28cropped%29.jpg/220px-Foods_%28cropped%29.jpg", Location = "Bellevue", Description="This is an item5 description." },
                new PickupItem { Id = Guid.NewGuid().ToString(), PictureURL = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/Foods_%28cropped%29.jpg/220px-Foods_%28cropped%29.jpg", Location = "Seattle", Description="This is an item6 description." },
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
