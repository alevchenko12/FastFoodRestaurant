using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class YourDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Meal> Meals { get; set; }
    public DbSet<MealProduct> MealProduct { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderMeal> OrderMeals { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer("Server=127.0.0.1,1433;Database=RestaurantDB;User Id=SA;Password=nastiaLord#11; Encrypt=False");
    }
}

public class OrderMeal
{
    [Key]
    public int OrderId { get; set; }
    public int MealId { get; set; }
    public int Amount { get; set; }
}

public class OrderMealsManager
{
    private YourDbContext context;

    public OrderMealsManager(YourDbContext dbContext)
    {
        context = dbContext;
    }

    /// <summary>
    /// Add Meal to Order in OrderMeals table 
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="mealName"></param>
    /// <param name="amount"></param>
    public void AddMealToOrder(int orderId, string mealName, int amount)
    {
        var order = context.Orders.FirstOrDefault(o => o.OrderId == orderId);
        var meal = context.Meals.FirstOrDefault(m => m.MealName == mealName);

        if (order != null && meal != null)
        {
            OrderMeal newOrderMeal = new OrderMeal
            {
                OrderId = order.OrderId,
                MealId = meal.MealId,
                Amount = amount
            };

            context.OrderMeals.Add(newOrderMeal);
            context.SaveChanges();
            //Console.WriteLine("Meal added to the order successfully.");
        }
        else
        {
            Console.WriteLine("Order or meal not found.");
        }
    }
}

public class Order
{
    [Key]
    public int OrderId { get; set; }
    public int ClientId { get; set; }
    public DateTime OrderDate { get; set; }
    public string ShipAddress { get; set; }
    public decimal Discount { get; set; }
    public bool ShipOpt { get; set; }
    public bool PayStatus { get; set; }
}

public class OrderManager
{
    private YourDbContext context;

    public OrderManager(YourDbContext dbContext)
    {
        context = dbContext;
    }

    /// <summary>
    /// Addind Order to Orders 
    /// based on clientId
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="shipAddress"></param>
    /// <param name="discount"></param>
    /// <param name="shipOption"></param>
    /// <param name="payStatus"></param>
    public void AddOrder(int clientId, string shipAddress, decimal discount, bool shipOption, bool payStatus)
    {
        var client = context.Clients.FirstOrDefault(c => c.ClientId == clientId);

        if (client != null)
        {
            Order newOrder = new Order
            {
                ClientId = clientId,
                OrderDate = DateTime.Now,
                ShipAddress = shipAddress,
                Discount = discount,
                ShipOpt = shipOption,
                PayStatus = payStatus
            };

            context.Orders.Add(newOrder);
            context.SaveChanges();
            //Console.WriteLine("Order added successfully.");
        }
        else
        {
            Console.WriteLine("Client not found.");
        }
    }

    /// <summary>
    /// Create file txt with list od ordered meals in order
    /// </summary>
    /// <param name="orderId"></param>
    public void ListOrderedMealsToFile(int orderId)
    {
        var mealsInOrder = context.Meals
                .Where(m => context.OrderMeals.Any(om => om.MealId == m.MealId && om.OrderId == orderId))
                .Select(m => new
                {
                    MealName = m.MealName,
                    Amount = context.OrderMeals
                        .Where(om => om.MealId == m.MealId && om.OrderId == orderId)
                        .Select(om => om.Amount)
                        .FirstOrDefault()
                })
                .ToList();

        using (StreamWriter writer = new StreamWriter($"OrderedMeals_Order_{orderId}.txt"))
        {
            writer.WriteLine($"Meals ordered for Order ID {orderId}:");
            if (mealsInOrder.Count > 0)
            {
                foreach (var mealInfo in mealsInOrder)
                {
                    writer.WriteLine($"- {mealInfo.MealName} (Amount: {mealInfo.Amount})");
                }
            }
            else
            {
                Console.WriteLine("No meals found for the specified OrderId.");
            }
        }

    }

    /// <summary>
    /// Create file txt with list of orders by client
    /// </summary>
    /// <param name="clientId"></param>
    public void ListOrderIdsForClient(int clientId)
    {
        var orderIds = context.Orders
            .Where(o => o.ClientId == clientId)
            .Select(o => o.OrderId)
            .ToList();
        using (StreamWriter writer = new StreamWriter($"Orders_For_Client{clientId}.txt"))
        {
            if (orderIds.Count > 0)
            {
                writer.WriteLine($"Order IDs for Client ID {clientId}:");
                foreach (var orderId in orderIds)
                {
                    writer.WriteLine(orderId);
                }
            }
            else
            {
                Console.WriteLine($"No orders found for Client ID {clientId}.");
            }
        }
    }

    /// <summary>
    /// Calulates total sum of ordered meals for order (client)
    /// </summary>
    /// <param name="orderId"></param>
    public void CalculateOrderTotalCost(int orderId)
    {
        var totalCost = context.OrderMeals
            .Where(om => om.OrderId == orderId)
            .Select(om => new
            {
                MealId = om.MealId,
                Amount = om.Amount
            })
            .ToList()
            .Sum(om => context.Meals
                .Where(m => m.MealId == om.MealId)
                .Select(m => m.MealPrice * om.Amount)
                .FirstOrDefault()
            );

        Console.WriteLine("Total cost for Order Id:" + orderId + " = " + totalCost);
    }

    /// <summary>
    /// Calculate total cost of ordered meals on day (income)
    /// </summary>
    /// <param name="targetDate"></param>
    public void CalculateTotalCostForDay(DateTime targetDate)
    {
        var totalCost = context.OrderMeals
            .Where(om => context.Orders.Any(o => o.OrderId == om.OrderId && o.OrderDate.Date == targetDate.Date))
            .Select(om => new
            {
                MealId = om.MealId,
                Amount = om.Amount
            })
            .ToList()
            .Sum(om => context.Meals
                .Where(m => m.MealId == om.MealId)
                .Select(m => m.MealPrice * om.Amount)
                .FirstOrDefault()
            );

        Console.WriteLine("Income for Restaurant on " + targetDate.ToString() + " = " + totalCost);
    }
}

public class MealProduct
{
    [Key]
    public int MealProdId { get; set; }
    public int MealId { get; set; }
    public int ProdId { get; set; }
    public int ProdAmount { get; set; }
}

public class MealProductManager
{
    private YourDbContext context;

    public MealProductManager(YourDbContext dbContext)
    {
        context = dbContext;
    }

    /// <summary>
    /// Add product to meal in MealProduct
    /// if Product is already in Meal update weight
    /// if not add Product to Meal 
    /// </summary>
    /// <param name="mealName"></param>
    /// <param name="productName"></param>
    /// <param name="prodAmount"></param>
    public void AddProductToMeal(string mealName, string productName, int prodAmount)
    {
        var meal = context.Meals.FirstOrDefault(m => m.MealName == mealName);

        if (meal != null)
        {
            var product = context.Products.FirstOrDefault(p => p.ProdName == productName);

            if (product != null)
            {
                var existingMealProduct = context.MealProduct.FirstOrDefault(mp => mp.MealId == meal.MealId && mp.ProdId == product.ProdId);

                if (existingMealProduct != null)
                {
                    existingMealProduct.ProdAmount = prodAmount;
                }
                else
                {
                    MealProduct newMealProduct = new MealProduct
                    {
                        MealId = meal.MealId,
                        ProdId = product.ProdId,
                        ProdAmount = prodAmount
                    };

                    context.MealProduct.Add(newMealProduct);
                }

                context.SaveChanges();
                //Console.WriteLine("Product added to the meal successfully.");
            }
            else
            {
                Console.WriteLine("Product not found.");
            }
        }
        else
        {
            Console.WriteLine("Meal not found.");
        }
    }

    /// <summary>
    /// Remove Product from Meal 
    /// if Product is in Meal 
    /// </summary>
    /// <param name="mealName"></param>
    /// <param name="productName"></param>
    public void RemoveProductFromMeal(string mealName, string productName)
    {
        var meal = context.Meals.FirstOrDefault(m => m.MealName == mealName);

        if (meal != null)
        {
            var product = context.Products.FirstOrDefault(p => p.ProdName == productName);

            if (product != null)
            {
                var mealProduct = context.MealProduct.FirstOrDefault(mp => mp.MealId == meal.MealId && mp.ProdId == product.ProdId);

                if (mealProduct != null)
                {
                    context.MealProduct.Remove(mealProduct);
                    context.SaveChanges();
                    //Console.WriteLine("Product removed from the meal successfully.");
                }
                else
                {
                    Console.WriteLine("Product is not part of the meal.");
                }
            }
            else
            {
                Console.WriteLine("Product not found.");
            }
        }
        else
        {
            Console.WriteLine("Meal not found.");
        }
    }
}

public class Product
{
    [Key]
    public int ProdId { get; set; }
    public string ProdName { get; set; }
    public decimal ProdWeightG { get; set; }
    public decimal ProdPrice { get; set; }
}

public class ProductManager
{
    private YourDbContext context;

    public ProductManager(YourDbContext dbContext)
    {
        context = dbContext;
    }

    /// <summary>
    /// Add or Update Product to Products 
    /// Add Product if it doesnt exist 
    /// Update Product if it already exists 
    /// (in that case weight improves and price is average)
    /// </summary>
    /// <param name="productName"></param>
    /// <param name="weightToAdd"></param>
    /// <param name="newProductPrice"></param>
    public void AddOrUpdateProduct(string productName, decimal weightToAdd, decimal newProductPrice)
    {
        var existingProduct = context.Products.FirstOrDefault(p => p.ProdName == productName);

        if (existingProduct != null)
        {
            existingProduct.ProdWeightG += weightToAdd;
            existingProduct.ProdPrice = (existingProduct.ProdPrice + newProductPrice) / 2;
        }
        else
        {
            Product newProduct = new Product
            {
                ProdName = productName,
                ProdWeightG = weightToAdd,
                ProdPrice = newProductPrice
            };

            context.Products.Add(newProduct);
        }

        context.SaveChanges();
    }

    /// <summary>
    /// Remove Product from Products
    /// Remove completely (if weight = current weight)
    /// Reduce wight withput deleting (current weight - weight)
    /// </summary>
    /// <param name="productName"></param>
    /// <param name="weightToRemove"></param>
    public void DeleteProduct(string productName, decimal weightToRemove)
    {
        var productToDelete = context.Products.FirstOrDefault(p => p.ProdName == productName);
        if (productToDelete != null)
        {
            if (weightToRemove >= productToDelete.ProdWeightG)
            {
                productToDelete.ProdWeightG = 0;
            }
            else
            {
                productToDelete.ProdWeightG -= weightToRemove;
            }

            context.SaveChanges();
            //Console.WriteLine("Product updated successfully.");
        }
        else
        {
            Console.WriteLine("Product not found.");
        }
    }

    /// <summary>
    /// Create list of products to buy from Products table 
    /// Products with 0 weight 
    /// </summary>
    public void ProductsToBuyFile()
    {
        var productsWithZeroWeight = context.Products
            .Where(p => p.ProdWeightG == 0)
            .ToList();

        if (productsWithZeroWeight.Count > 0)
        {
            using (StreamWriter writer = new StreamWriter("ProductsToBuy.txt"))
            {
                foreach (var product in productsWithZeroWeight)
                {
                    writer.WriteLine($"Product ID: {product.ProdId}");
                    writer.WriteLine($"Product Name: {product.ProdName}");
                    writer.WriteLine($"Product Weight (in grams): {product.ProdWeightG}");
                    writer.WriteLine($"Product Price ($ per 100g): {product.ProdPrice:C}");
                    writer.WriteLine();
                }
            }
            //Console.WriteLine("Zero weight products saved to ZeroWeightProducts.txt.");
        }
        else
        {
            Console.WriteLine("No products with zero weight found.");
        }
    }

    /// <summary>
    /// Calculates total cost of all products (outcome)
    /// </summary>
    public void CalculateTotalPriceOfProducts()
    {
        var totalCost = context.Products
            .Select(p => (p.ProdWeightG * p.ProdPrice) / 100)
            .Sum();

        Console.WriteLine("Total outcome for Restaurant = " + totalCost);
    }
}

public class Meal
{
    [Key]
    public int MealId { get; set; }
    public string MealName { get; set; }
    public decimal MealPrice { get; set; }
    public decimal MealWeight { get; set; }
}

public class MealManager
{
    private YourDbContext context;

    public MealManager(YourDbContext dbContext)
    {
        context = dbContext;
    }

    /// <summary>
    /// Add or updape Meal in Meals 
    /// Add Meal if it doesnt exists
    /// Update weight and price if Meal exists
    /// </summary>
    /// <param name="mealName"></param>
    /// <param name="newMealWeight"></param>
    /// <param name="newMealPrice"></param>
    public void AddOrUpdateMeal(string mealName, decimal newMealWeight, decimal newMealPrice)
    {
        var existingMeal = context.Meals.FirstOrDefault(m => m.MealName == mealName);

        if (existingMeal != null)
        {
            existingMeal.MealWeight = newMealWeight;
            existingMeal.MealPrice = newMealPrice;
        }
        else
        {
            Meal newMeal = new Meal
            {
                MealName = mealName,
                MealWeight = newMealWeight,
                MealPrice = newMealPrice
            };

            context.Meals.Add(newMeal);
        }

        context.SaveChanges();
    }

    //DELETE CORESPONDING ROWS IS JOIN TABLE
    /// <summary>
    /// Delete Meal from Meals if it exists
    /// </summary>
    /// <param name="mealName"></param>
    public void DeleteMeal(string mealName)
    {
        var mealToDelete = context.Meals.FirstOrDefault(m => m.MealName == mealName);
        if (mealToDelete != null)
        {
            context.Meals.Remove(mealToDelete);
            context.SaveChanges();
            //Console.WriteLine("Meal updated successfully.");
        }
        else
        {
            Console.WriteLine("Meal not found.");
        }
    }
}

public class Client
{
    [Key]
    public int ClientId { get; set; }
    public string FirstLastName { get; set; }
    public string PhoneNumber { get; set; }
    public string EMail { get; set; }
    public string Address { get; set; }
    public DateTime BirthDay { get; set; }
}

public class ClientManager
{
    private YourDbContext context;

    public ClientManager(YourDbContext dbContext)
    {
        context = dbContext;
    }

    /// <summary>
    /// Add Client to Clients table 
    /// clients with similar names or surnames can exist
    /// </summary>
    /// <param name="firstLastName"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="email"></param>
    /// <param name="address"></param>
    /// <param name="birthDay"></param>
    public void AddClient(string firstLastName, string phoneNumber, string email, string address, DateTime birthDay)
    {
        Client newClient = new Client
        {
            FirstLastName = firstLastName,
            PhoneNumber = phoneNumber,
            EMail = email,
            Address = address,
            BirthDay = birthDay
        };

        context.Clients.Add(newClient);
        context.SaveChanges();
        //Console.WriteLine("Client added successfully.");
    }

    /// <summary>
    /// Update info about Client in Clients table 
    /// based on their Id
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="firstLastName"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="email"></param>
    /// <param name="address"></param>
    /// <param name="birthDay"></param>
    public void UpdateClient(int clientId, string firstLastName, string phoneNumber, string email, string address, DateTime birthDay)
    {
        var clientToUpdate = context.Clients.FirstOrDefault(c => c.ClientId == clientId);

        if (clientToUpdate != null)
        {
            clientToUpdate.FirstLastName = firstLastName;
            clientToUpdate.PhoneNumber = phoneNumber;
            clientToUpdate.EMail = email;
            clientToUpdate.Address = address;
            clientToUpdate.BirthDay = birthDay;

            context.SaveChanges();
            //Console.WriteLine("Client information updated successfully.");
        }
        else
        {
            Console.WriteLine("Client not found.");
        }
    }

    /// <summary>
    /// Delete Client from Clients table 
    /// based on clients id 
    /// </summary>
    /// <param name="clientId"></param>
    public void DeleteClient(int clientId)
    {
        var clientToDelete = context.Clients.FirstOrDefault(c => c.ClientId == clientId);

        if (clientToDelete != null)
        {
            context.Clients.Remove(clientToDelete);
            context.SaveChanges();
            //Console.WriteLine("Client deleted successfully.");
        }
        else
        {
            Console.WriteLine("Client not found.");
        }
    }
}

public class MenuManager
{
    private YourDbContext context;

    public MenuManager(YourDbContext dbContext)
    {
        context = dbContext;
    }

    /// <summary>
    /// Creates list of meals and products for them
    /// </summary>
    public void CreateMenu()
    {
        var meals = context.Meals.ToList();

        if (meals.Count > 0)
        {
            using (StreamWriter writer = new StreamWriter("Menu.txt"))
            {
                foreach (var meal in meals)
                {
                    writer.WriteLine($"Meal Name: {meal.MealName}");
                    writer.WriteLine($"Price: {meal.MealPrice:C}");
                    writer.WriteLine($"Weight: {meal.MealWeight}g");
                    writer.WriteLine("Products:");

                    var mealProducts = context.MealProduct.Where(mp => mp.MealId == meal.MealId).ToList();

                    if (mealProducts.Count > 0)
                    {
                        foreach (var mealProduct in mealProducts)
                        {
                            var product = context.Products.FirstOrDefault(p => p.ProdId == mealProduct.ProdId);
                            writer.WriteLine($"- {product.ProdName}: {mealProduct.ProdAmount} units");
                        }
                    }
                    else
                    {
                        writer.WriteLine("No products for this meal.");
                    }

                    writer.WriteLine();
                }
            }

            //Console.WriteLine("Menu saved to Menu.txt.");
        }
        else
        {
            Console.WriteLine("No meals found.");
        }
    }
}

class Program
{
    static void Main()
    {
        using (var context = new YourDbContext())
        {
            ProductManager productManager = new ProductManager(context);
            productManager.AddOrUpdateProduct("Potatoes", 500m, 5.70m);
            //productManager.DeleteProduct("Potatoes", 500m);

            //productManager.ProductsToBuyFile();
            //productManager.CalculateTotalPriceOfProducts();

            //MealManager mealManager = new MealManager(context);
            //mealManager.AddOrUpdateMeal("Salad", 250.00m, 45.50m);
            //mealManager.DeleteMeal("Salad");

            //MealProductManager mealProductManager = new MealProductManager(context);
            //mealProductManager.AddProductToMeal("Salad", "Cucumber", 55.00m);
            //mealProductManager.AddProductToMeal("Salad", "Tomato", 155.00m);
            //mealProductManager.AddProductToMeal("Salad", "Tomato", 85.00m);
            //mealProductManager.RemoveProductFromMeal("Dessert", "Apple");

            //MenuManager menu = new MenuManager(context);
            //menu.CreateMenu();

            //ClientManager clientManager = new ClientManager(context);
            //clientManager.AddClient("Levchenko Anastasiia", "071 633 21 09", "levchenko.nastia.gmail.com", "Neighborhood", new DateTime(2017, 6, 17));
            //clientManager.UpdateClient(2, "Levchenko Anastasiia", "082 391 54 92", "levchenko.anastasiia.gmail.com", "City Plaza", new DateTime(2005, 5, 29));
            //clientManager.DeleteClient(4);

            //OrderManager orderManager = new OrderManager(context);
            //orderManager.AddOrder(5, "Arkadia", 0.05m, true, true);
            //orderManager.AddOrder(3, "Neighborhood", 0.05m, false, false);
            //orderManager.AddOrder(2, "Post Square", 0.05m, true, true);
            //orderManager.AddOrder(5, "Cafe", 0.05m, true, false);
            //orderManager.AddOrder(7, "River Mall", 0.05m, false, true);

            //orderManager.ListOrderedMealsToFile(3);
            //orderManager.ListOrderIdsForClient(5);
            //orderManager.CalculateOrderTotalCost(3);
            //orderManager.CalculateTotalCostForDay(new DateTime(2023, 10, 21));

            //OrderMealsManager orderMealsManager = new OrderMealsManager(context);
            //orderMealsManager.AddMealToOrder(1, "Sandwich", 2);
            //orderMealsManager.AddMealToOrder(2, "Salad", 1);
            //orderMealsManager.AddMealToOrder(3, "Sandwich", 3);
            //orderMealsManager.AddMealToOrder(3, "Dessert", 2);
            //orderMealsManager.AddMealToOrder(1, "Breakfast", 3); 
            //orderMealsManager.AddMealToOrder(1, "Salad", 1);

            Console.WriteLine("Completed");
        }
    }
}