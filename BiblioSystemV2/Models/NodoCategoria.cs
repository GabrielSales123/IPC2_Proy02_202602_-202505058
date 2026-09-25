
namespace BiblioSystemV2.Models
{
    public class NodoCategoria
    {
        private Categoria? categoria;
        private NodoCategoria? padre;
        private NodoCategoria? izquierda;
        private NodoCategoria? derecha;
        private String? llave; 

        public NodoCategoria(Categoria categoria)
        {
            this.categoria = categoria; 
            this.padre = null; 
            this.izquierda = null; 
            this.derecha = null;
            this.llave = categoria.getNombre();
        }

        public Categoria? getCategoria()
        {
            return this.categoria;
        }

        public void setCategoria(Categoria categoria)
        {
            this.categoria = categoria;
        }

        public NodoCategoria? getPadre()
        {
            return this.padre;
        }

        public void setPadre(NodoCategoria? padre)
        {
            this.padre = padre;
        }

        public NodoCategoria? getIzquierda()
        {
            return this.izquierda;
        }

        public void setIzquierda(NodoCategoria? izquierda)
        {
            this.izquierda = izquierda;
        }

        public NodoCategoria? getDerecha()
        {
            return this.derecha;
        }

        public void setDerecha(NodoCategoria? derecha)
        {
            this.derecha = derecha;
        }

        public String? getLlave()
        {
            return this.llave;
        }

        public void setLlave(String? llave)
        {
            this.llave = llave;
        }
    }
}