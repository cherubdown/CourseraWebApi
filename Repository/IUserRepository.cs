public interface IUserRepository
{
    List<User> GetAll();
    User GetById(int id);
    void Post(User user);
    void Put(int id, User user);
    void Delete(int id);
}