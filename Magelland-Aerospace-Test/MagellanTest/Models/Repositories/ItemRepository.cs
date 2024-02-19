using MagellanTest.Models.Types;
using Npgsql;

namespace MagellanTest.Models.Repositories;

public interface IItemRepository
{
    Task<ItemRecord?> GetByIdAsync(int id);
    Task<CreateItemResponse> AddAsync(Item item);
    Task<GetTotalCostByItemNameResponse> GetTotalCostByNameAsync(string itemName);
}

public class ItemRepository : IItemRepository
{
    private readonly NpgsqlConnection _connection = new("Host=localhost;Username=postgres;Password=123456789;Database=part");

    public async Task<ItemRecord?> GetByIdAsync(int id)
    {
        try
        {
            _connection.Open();

            await using var transaction = await _connection.BeginTransactionAsync();
            
            var command = new NpgsqlCommand("SELECT * FROM item WHERE Id = @Id", _connection);
            command.Parameters.AddWithValue("Id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ItemRecord
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    ParentItemId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                    Cost = reader.GetInt32(3),
                    ReqDate = reader.GetDateTime(4)
                };
            }
            
            await _connection.CloseAsync();
            
            return null;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
        finally
        {
            _connection.Dispose();
        }
    }

    public async Task<CreateItemResponse> AddAsync(Item item)
    {
        try
        {
            _connection.Open();

            await using var transaction = await _connection.BeginTransactionAsync();
            
            var command = new NpgsqlCommand("INSERT INTO item (item_name, parent_item, cost, req_date) VALUES (@Name, @ParentItem, @Cost, @ReqDate) RETURNING Id", _connection);
            command.Parameters.AddWithValue("Name", item.Name);
            if (item.ParentItemId != null)
            {
                command.Parameters.AddWithValue("ParentItem", item.ParentItemId);
            }
            else
            {
                command.Parameters.AddWithValue("ParentItem", DBNull.Value);  
            }
            command.Parameters.AddWithValue("Cost", item.Cost);
            command.Parameters.AddWithValue("ReqDate", item.ReqDate);

            var id = await command.ExecuteScalarAsync();

            await transaction.CommitAsync();
            
            await _connection.CloseAsync();
            
            return new CreateItemResponse
            {
                Id = Convert.ToInt32(id)
            };
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
        finally
        {
            _connection.Dispose();
        }
    }

    public async Task<GetTotalCostByItemNameResponse> GetTotalCostByNameAsync(string itemName)
    {
        try
        {
            _connection.Open();

            await using var transaction = await _connection.BeginTransactionAsync();
            
            var command = new NpgsqlCommand($"SELECT Get_Total_Cost('{itemName}')", _connection);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new GetTotalCostByItemNameResponse
                {
                  TotalCost = reader.IsDBNull(0) ? null : reader.GetInt32(0),
                };
            }
            
            await _connection.CloseAsync();

            return new GetTotalCostByItemNameResponse
            {
                TotalCost = null
            };
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
        finally
        {
            _connection.Dispose();
        }
    }
}

public class ItemRecord
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public int? ParentItemId { get; set; }
    
    public int Cost { get; set; }
    
    public DateTime ReqDate { get; set; }
}