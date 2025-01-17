﻿using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repository.IRepository;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApiPeliculas.Repository
{
    public class CategoriaRepository : ICategoriaRepository
    {

        private readonly AplicationDBContext _db;
        public CategoriaRepository(AplicationDBContext db)
        {
            _db = db;  
        }


        public bool ActualizarCategoria(Categoria categoria)
        {
           categoria.FechaCreacion = DateTime.Now;
            _db.Categoria.Update(categoria);
            return Guardar();
        }

        public bool BorrarCategoria(Categoria categoria)
        {
           _db.Categoria.Remove(categoria);
            return Guardar();   
        }

        public bool CrearCategoria(Categoria categoria)
        {
            categoria.FechaCreacion = DateTime.Now;
            _db.Categoria.Add(categoria);
            return Guardar();
        }

        public bool ExisteCategoria(string nombre)
        {
            bool valor = _db.Categoria.Any(c=>c.Nombre.ToLower().Trim()== nombre.ToLower().Trim());
            return valor;
        }

        public bool ExisteCategoria(int id)
        {
            bool valor = _db.Categoria.Any(c => c.Id ==id);
            return valor;
        }

        public Categoria GetCategoria(int categoriaId)
        {
            return _db.Categoria.FirstOrDefault(c => c.Id == categoriaId);
        }

        public ICollection<Categoria> GetCategorias()
        {
            return _db.Categoria.OrderBy(c => c.Nombre).ToList();
        }

        public bool Guardar()
        {
           return _db.SaveChanges() >= 0 ? true : false;
        }
    }
}
