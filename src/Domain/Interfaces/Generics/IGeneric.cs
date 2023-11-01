namespace Domain.Interfaces.Generics
{
    public interface IGeneric<T> where T : class
    {
        Task Add(T item);
        Task Update(T item);
        Task Delete(T item);
        Task<T> GetEntityById(int id);
        Task<IList<T>> List();
    }
}
