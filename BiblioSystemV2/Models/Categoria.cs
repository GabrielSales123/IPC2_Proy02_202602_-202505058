
namespace BiblioSystemV2.Models{
    public class Categoria
    {
        private string? nombre;
        private Categoria? padre;
        private ArbolCategorias? subcategorias;
        private ArbolLibro? libros;
        
        public Categoria(String nombre)
        {
            this.nombre = nombre;
            this.padre = null; 
            this.subcategorias = new ArbolCategorias();
            this.libros = new ArbolLibro();
        }

        public string? getNombre()
        {
            return this.nombre;
        }

        public void setNombre(string? nombre)
        {
            this.nombre = nombre;
        }

        public Categoria? getPadre()
        {
            return this.padre;
        }

        public void setPadre(Categoria? padre)
        {
            this.padre = padre;
        }

        public ArbolCategorias? getSubcategorias()
        {
            return this.subcategorias;
        }

        public void setSubcategorias(ArbolCategorias? subcategorias)
        {
            this.subcategorias = subcategorias;
        }

        public ArbolLibro? getLibros()
        {
            return this.libros;
        }

        public void setLibros(ArbolLibro? libros)
        {
            this.libros = libros;
        }
    }
    
}