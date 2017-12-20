using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSL.Models;

namespace OSL
{
    public class MockDataStore : IDataStore<Donation>
    {
        List<Donation> items;

        public MockDataStore()
        {
            items = new List<Donation>();
            var mockItems = new List<Donation>
            {
                new Donation { Id = 10, PictureUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/Foods_%28cropped%29.jpg/220px-Foods_%28cropped%29.jpg"},
                new Donation { Id = 11, PictureUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/Foods_%28cropped%29.jpg/220px-Foods_%28cropped%29.jpg"},
                new Donation { Id = 12, PictureUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/Foods_%28cropped%29.jpg/220px-Foods_%28cropped%29.jpg"},
                new Donation { Id = 13, PictureUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/Foods_%28cropped%29.jpg/220px-Foods_%28cropped%29.jpg"},
                new Donation { Id = 14, PictureUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/Foods_%28cropped%29.jpg/220px-Foods_%28cropped%29.jpg"},
                new Donation { Id = 15, PictureUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/Foods_%28cropped%29.jpg/220px-Foods_%28cropped%29.jpg"},
            };

            foreach (var item in mockItems)
            {
                items.Add(item);
            }
        }

        public async Task<bool> AddPickupItemAsync(Donation item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdatePickupItemAsync(Donation item)
        {
            var _item = items.Where((Donation arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(_item);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeletePickupItemAsync(string id)
        {
            var _item = items.Where((Donation arg) => arg.Id.ToString() == id).FirstOrDefault();
            items.Remove(_item);

            return await Task.FromResult(true);
        }

        public async Task<Donation> GetPickupItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id.ToString() == id));
        }

        public async Task<IEnumerable<Donation>> GetPickupItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }

        public async Task<IEnumerable<Donation>> GetFilteredItemsAsync(int range, double? Lat, double? Long, bool forceRefres = false)
        {
            return await Task.FromResult(items);
        }
    }
}
