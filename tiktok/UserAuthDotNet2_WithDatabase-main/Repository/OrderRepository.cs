using System.Text;
using Dapper;
using static UserAuthDotBet2_WithDatabase.AuthController;

namespace UserAuthDotBet2_WithDatabase.Repositories;

public interface IOrderRepository
{
    public Task<List<Orders>> GetOrders(int UserId);
    public Task<bool> placeOrders(List<Cart> cart, int userId, int PaymentTypeId);
}


public class OrderRepository : BaseRepository, IOrderRepository
{
    public OrderRepository(IConfiguration config) : base(config)
    {
    }

    public async Task<List<Orders>> GetOrders(int UserId)
    {
        var query = "SELECT * FROM orders WHERE user_id = @UserId";
        var con = NewConnection;
        var res = await con.QueryAsync<Orders>(query, new
        {
            UserId = UserId
        });
        return res.AsList();


    }

    public async Task<bool> placeOrders(List<Cart> cart, int userId, int PaymentTypeId)
    {
        var orderId = new Random().Next(1000, 10000);
        Console.WriteLine(orderId);

        using (var con = NewConnection)
        {
            // Create a StringBuilder to build the SQL query
            var insertSql = new StringBuilder($"INSERT INTO orders (order_id, product_id, quantity, order_price, user_id, payment_type_id) VALUES");
            // Iterate through cart items to construct the value sets
            foreach (var cartItem in cart)
            {
                insertSql.Append($" ({orderId}, {cartItem.ProductId}, {cartItem.Quantity}, {(cartItem.Quantity) * cartItem.Price}, {userId}, {PaymentTypeId}),");
            }
            insertSql.Length--; // Remove the last character (comma)
            insertSql.Append("RETURNING *");
            // Remove the trailing comma


            // Convert the StringBuilder to a string
            string finalSql = insertSql.ToString();

            try
            {
                var res = await con.QueryAsync<Orders>(finalSql);

                if (res.Count() > 0)
                {
                    var deleteQuery = "DELETE FROM cart WHERE user_id = @UserId";
                    // Delete only the selected items (Change it in the future)

                    var response = await con.ExecuteAsync(deleteQuery, new { UserId = userId }) > 0;

                    if (!response)
                    {
                        throw new Exception("Failed to delete cart items");
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log it) and return an empty list to indicate failure
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }
    }

}
