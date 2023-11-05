using System;
using System.Collections.Generic;

namespace MiniPizzeria
{
    public enum MenuCode
    {
        // Перелічення кодів меню, що представляє можливі товари в міні-піцерії.

        PizzaClassic = 1, // Классична 
        PizzaTropical = 2, // Тропічна
        PizzaVeggie = 3, // Веганська
        PizzaHawaiian = 4, // Гавайська
        DrinkSoda = 5, // Кола або содова
        DrinkHerbalTea = 6, // Чай с травами
        DrinkEspresso = 7 // Кава Еспрессо
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Ціни на товари в меню.
            var prices = new Dictionary<MenuCode, decimal>
            {
                [MenuCode.PizzaClassic] = 7.99m,
                [MenuCode.PizzaTropical] = 8.99m,
                [MenuCode.PizzaVeggie] = 9.99m,
                [MenuCode.PizzaHawaiian] = 10.99m,
                [MenuCode.DrinkSoda] = 1.99m,
                [MenuCode.DrinkHerbalTea] = 2.50m,
                [MenuCode.DrinkEspresso] = 3.00m
            };

            // Словник для збереження замовлення клієнта.
            var order = new Dictionary<MenuCode, int>();
            // Загальна сума замовлення.
            decimal totalAmount = 0;
            // Кількість замовлених піц.
            int pizzaCount = 0;
            // "Флажок" для визначення, чи додавати ще товари до замовлення.
            bool addMoreItems = true;

            Console.WriteLine("Welcome to the Mini-Pizzeria!");

            // Виводимо меню і ціни.
            foreach (var item in prices)
            {
                Console.WriteLine($"{(int)item.Key}. {item.Key} - ${item.Value}");
            }
            // Процес замовлення. Основний цикл
            while (addMoreItems)
            {
                try
                {
                    Console.Write("Please enter the code of the product you want to order: ");
                    if (!Enum.TryParse(Console.ReadLine(), out MenuCode code) || !Enum.IsDefined(typeof(MenuCode), code))
                    {
                        Console.WriteLine("Invalid code, please try again.");
                        continue;
                    }

                    Console.Write($"Enter the number of {code}s you want to order: ");
                    if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
                    {
                        Console.WriteLine("Invalid quantity, please enter a positive number.");
                        continue;
                    }

                    decimal price = prices[code];
                    // Додаткові дії, якщо замовлена піца.
                    if (code <= MenuCode.PizzaHawaiian)
                    {
                        pizzaCount += quantity;
                    }

                    // Застосування знижки на напої при замовленні більше трьох.
                    if (code > MenuCode.PizzaHawaiian && quantity > 3)
                    {
                        price *= 0.85m; // 15% знижка на напої
                    }

                    // Додавання товару до замовлення.
                    if (order.ContainsKey(code))
                    {
                        order[code] += quantity;
                    }
                    else
                    {
                        order.Add(code, quantity);
                    }

                    // Проміжний розрахунок загальної суми для визначення кількості безкоштовних піц.
                    totalAmount += price * quantity;

                    Console.Write("Add more items to the order or print a receipt? (add/print): ");
                    string choice = Console.ReadLine().ToLower();
                    addMoreItems = choice == "add";

                    if (choice == "print")
                    {
                        break;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("The format of the input is not valid. Please enter the correct format.");
                }
                catch (OverflowException)
                {
                    Console.WriteLine("The number is too large or too small. Please enter a valid number.");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }

            try
            {

                // Застосування знижок.
                // Знижка за безкоштовні піци.
                int freePizzas = pizzaCount / 5;
                decimal pizzaDiscount = freePizzas * prices[MenuCode.PizzaClassic];
                totalAmount -= pizzaDiscount;

                // Даємо знижка на все замовлення.
                if (totalAmount > 50)
                {
                    totalAmount *= 0.8m; // 20% знижка на загальну суму замовлення, якщо вона більше $50.
                }

                // Вивод чеку.
                Console.WriteLine("\nReceipt for payment:");
                foreach (var item in order)
                {
                    decimal itemTotalPrice = prices[item.Key] * item.Value;
                    // Коригування ціни, якщо є безкоштовні піци.
                    if (item.Key <= MenuCode.PizzaHawaiian && freePizzas > 0)
                    {
                        int deductablePizzas = Math.Min(freePizzas, item.Value);
                        itemTotalPrice -= deductablePizzas * prices[MenuCode.PizzaClassic];
                        freePizzas -= deductablePizzas; // Тут  віднімаємо кількість безкоштовних піц.
                    }
                    Console.WriteLine($"{item.Key} - {item.Value} x ${prices[item.Key]} = ${itemTotalPrice}");
                }
                Console.WriteLine($"Total to be paid: ${totalAmount:F2}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while calculating the total: {ex.Message}");
            }
        }
    }
}
