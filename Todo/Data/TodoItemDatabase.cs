using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using acData = Microsoft.AppCenter.Data;

namespace Todo
{    
	public class TodoItemDatabase
	{
        public TodoItemDatabase(string dbPath)
		{
		}

        public async Task<List<TodoItem>> GetItemsAsync()
        {
            var todos = new List<TodoItem>();
            var pages = await acData.Data.ListAsync<TodoItem>(acData.DefaultPartitions.UserDocuments);

            do
            {
                var wrappedItems = pages.CurrentPage.Items;
                todos.AddRange(wrappedItems.Select(td => td.DeserializedValue));
                if (pages.HasNextPage)
                {
                    await pages.GetNextPageAsync();
                }
                else
                {
                    break;
                }
            } while (true);

            return todos;
        }

        public async Task<TodoItem> GetItemAsync(string id)
		{
            var wrappedItem = await acData.Data.ReadAsync<TodoItem>(id, acData.DefaultPartitions.UserDocuments);
            return wrappedItem.DeserializedValue;
        }

        public async Task<string> SaveItemAsync(TodoItem item)
		{
            if(item.ID == null)
            {
                item.ID = Guid.NewGuid().ToString();
                await acData.Data.CreateAsync(item.ID, item, acData.DefaultPartitions.UserDocuments);
            } 
            else
            {
                await acData.Data.ReplaceAsync(item.ID, item, acData.DefaultPartitions.UserDocuments);
            }
            return item.ID;
		}

		public async Task<string> DeleteItemAsync(TodoItem item)
		{
            var wrappedItem = await acData.Data.DeleteAsync<TodoItem>(item.ID, acData.DefaultPartitions.UserDocuments);
            return item.ID;
        }
    }
}

