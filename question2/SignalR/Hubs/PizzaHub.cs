using Microsoft.AspNetCore.SignalR;
using SignalR.Services;

namespace SignalR.Hubs
{
    public class PizzaHub : Hub
    {
        private readonly PizzaManager _pizzaManager;

        public PizzaHub(PizzaManager pizzaManager) {
            _pizzaManager = pizzaManager;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            _pizzaManager.AddUser();

            await Clients.All.SendAsync("UpdateNbUsers", _pizzaManager.NbConnectedUsers);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnConnectedAsync();

            _pizzaManager.RemoveUser();

            await Clients.All.SendAsync("UpdateNbUsers", _pizzaManager.NbConnectedUsers);
        }

        public async Task SelectChoice(PizzaChoice choice)
        {
            string grp1 = _pizzaManager.GetGroupName(choice);
            int money = _pizzaManager.Money[(int)choice];
            int price = _pizzaManager.PIZZA_PRICES[(int)choice];
            int nbPizzas = _pizzaManager.NbPizzas[(int)choice];

            int[] grp2 = [money, nbPizzas];

            await Groups.AddToGroupAsync(Context.ConnectionId, grp1);

            await Clients.Group(grp1).SendAsync("UpdateNbPizzasAndMoney", grp2);
            await Clients.All.SendAsync("UpdatePizzaPrice", price);
        }

        public async Task UnselectChoice(PizzaChoice choice)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, _pizzaManager.GetGroupName(choice));
        }

        public async Task AddMoney(PizzaChoice choice)
        {
            string grp1 = _pizzaManager.GetGroupName(choice);

            _pizzaManager.IncreaseMoney(choice);

            await Clients.Group(grp1).SendAsync("UpdateMoney", _pizzaManager.Money[(int)choice]);
        }

        public async Task BuyPizza(PizzaChoice choice)
        {
            _pizzaManager.BuyPizza(choice);

            string grp1 = _pizzaManager.GetGroupName(choice);
            int money = _pizzaManager.Money[(int)choice];
            int nbPizza = _pizzaManager.NbPizzas[(int)choice];

            int[] grp2 = [money, nbPizza];

            await Groups.AddToGroupAsync(Context.ConnectionId, grp1);

            await Clients.Group(grp1).SendAsync("UpdateNbPizzasAndMoney", grp2);
        }
    }
}
