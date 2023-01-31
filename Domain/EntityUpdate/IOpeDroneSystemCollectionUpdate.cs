namespace Domain.DTO;

public interface IOpeDroneSystemCollectionUpdate<out TUpdate>
{
    public TUpdate Update();
}