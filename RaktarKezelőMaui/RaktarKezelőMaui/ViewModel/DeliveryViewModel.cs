using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DataBase;

public class DeliveryViewModel
{
    private readonly DeliveryRepository _repository;

    public ObservableCollection<Delivery> Deliveries { get; } = new();

    public DeliveryViewModel(DeliveryRepository repository)
    {
        _repository = repository;
    }

    public async Task LoadDeliveriesAsync()
    {
        var deliveries = await _repository.GetAllDeliveriesAsync();
        Deliveries.Clear();
        foreach (var delivery in deliveries)
            Deliveries.Add(delivery);
    }

    public async Task AddDeliveryAsync(Delivery delivery)
    {
        await _repository.AddDeliveryAsync(delivery);
        await LoadDeliveriesAsync();
    }

    public async Task DeleteDeliveryAsync(int id)
    {
        await _repository.DeleteDeliveryAsync(id);
        await LoadDeliveriesAsync();
    }
}