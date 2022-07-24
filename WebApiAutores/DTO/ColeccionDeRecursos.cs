namespace WebApiAutores.DTO
{
    public class ColeccionDeRecursos<T>: Recurso where T: Recurso
    {
        public List<T> Valores { get; set; }
    }
}
