// See https://aka.ms/new-console-template for more information
using ClientOrleans;
using GrainInterfaces.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

Console.WriteLine("Publicação de ativos para o Teste do Orleans!");


var host = await OrleansConnection.StartOrleansClient();
var orleansClient = host.Services.GetRequiredService<IClusterClient>();

Random random = new Random();
ConcurrentDictionary<string, bool> loop = new ConcurrentDictionary<string, bool>();

try
{
    string _exit = string.Empty;

    while (_exit != "S") 
    {
        Console.WriteLine("");
        Console.WriteLine("");
        Console.WriteLine("");

        Console.WriteLine("Digite o código do ativo!");

        string symbol = Console.ReadLine();

        if (loop.ContainsKey(symbol)) 
        {
            Console.WriteLine("");
            Console.WriteLine($"Deseja para a publicação desse symbol: {symbol} se sim digite 1");

            string action = Console.ReadLine();

            if (1 == int.Parse(action)) 
            {
                loop[symbol] = false;
            }
        }
        else 
        {
            loop[symbol] = true;

            var _ = () => Func(symbol);
            _.Invoke();
        }

        Console.WriteLine("");
        Console.WriteLine("");
        Console.WriteLine("");

        Console.WriteLine("Digite 'S' para sair ou qualquer outra teclara para continuar!");
        _exit = Console.ReadLine();
    }
}
finally 
{
    await host.StopAsync();
}

async void Func(string symbol) 
{
    while (loop[symbol])
    {
        var iGrain = orleansClient.GetGrain<IStockGrain>(symbol);

        await iGrain.Set(new States.Models.StockState { Symbol = symbol, Price = (decimal)random.NextDouble() });

        await Task.Delay(500);
    }

    loop.TryRemove(symbol, out bool _);
}