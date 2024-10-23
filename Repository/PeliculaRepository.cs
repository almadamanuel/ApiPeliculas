using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repository.IRepository;
using System.Data.Entity;

namespace ApiPeliculas.Repository
{
    public class PeliculaRepository : IPeliculaRepository
    {

        private readonly AplicationDBContext _db;
        public PeliculaRepository(AplicationDBContext db)
        {
            _db = db;  
        }


        public bool ActualizarPelicula(Pelicula pelicula)
        {
           pelicula.FechaCreacion = DateTime.Now;
            _db.Pelicula.Update(pelicula);
            return Guardar();
        }

        public bool BorrarPelicula(Pelicula pelicula)
        {
           _db.Pelicula.Remove(pelicula);
            return Guardar();   
        }

        public ICollection<Pelicula> BuscarPelicula(string nombre)
        {
            IQueryable<Pelicula> query = _db.Pelicula;

            if (! string.IsNullOrEmpty(nombre))
            {
                query = query.Where(e => e.Nombre.Contains(nombre) || e.Descripcion.Contains(nombre));
            }

            return query.ToList();
        }

        public bool CrearPelicula(Pelicula pelicula)
        {
            pelicula.FechaCreacion = DateTime.Now;
            _db.Pelicula.Add(pelicula);
            return Guardar();
        }

        public bool ExistePelicula(string nombre)
        {
            bool valor = _db.Pelicula.Any(c=>c.Nombre.ToLower().Trim()== nombre.ToLower().Trim());
            return valor;
        }

        public bool ExistePelicula(int id)
        {
            bool valor = _db.Pelicula.Any(c => c.Id ==id);
            return valor;
        }

        public Pelicula GetPelicula(int peliculaId)
        {
            return _db.Pelicula.FirstOrDefault(c => c.Id == peliculaId);
        }

        public ICollection<Pelicula> GetPeliculas()
        {
            return _db.Pelicula.OrderBy(c => c.Nombre).ToList();
        }

        public ICollection<Pelicula> GetPeliculasEnCategoria(int catId)
        {
            return _db.Pelicula.Include(ca =>ca.Categoria).Where(ca =>ca.categoriaId == catId).ToList();
        }

        public bool Guardar()
        {
           return _db.SaveChanges() >= 0 ? true : false;
        }



    }
}
