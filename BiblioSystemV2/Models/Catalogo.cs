
namespace BiblioSystemV2.Models
{
    public class Catalogo
    {
        private ArbolCategorias? categorias;
        public Catalogo()
        {
            this.categorias = new ArbolCategorias();
        }

        public ArbolCategorias? getCategorias()
        {
            return this.categorias;
        }

        public void setCategorias(ArbolCategorias categorias)
        {
            this.categorias = categorias; 
        }
    }
}