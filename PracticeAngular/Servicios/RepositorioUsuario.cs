namespace PracticeAngular.Servicios
{
    public interface IRepositorioUsuario
    {
        int obtenerUsuario();
    }

    public class RepositorioUsuario : IRepositorioUsuario
    {
        public int obtenerUsuario()
        {
            return 1;
        }
      
    }
}
