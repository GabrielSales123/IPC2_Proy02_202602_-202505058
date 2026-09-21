
namespace BiblioSystemV2.Models
{
    public class NodoLibro
    {
        private Libro? libro;
        private NodoLibro? izquierda;
        private NodoLibro? derecha;
        private NodoLibro? padre; 
        private long? llave;

        public NodoLibro(Libro libro){
            this.izquierda = null; 
            this.derecha = null;
            this.padre = null;
            this.libro = libro;
            this.llave = libro.getISBN();
        }

        public Libro? getLibro()
        {
            return this.libro;
        }

        public void setLibro(Libro libro)
        {
            this.libro = libro;
        }

        public NodoLibro? getIzquierda()
        {
            return this.izquierda;
        }

        public void setIzquierda(NodoLibro? izquierda)
        {
            this.izquierda = izquierda;
        }

        public NodoLibro? getDerecha()
        {
            return this.derecha;
        }

        public void setDerecha(NodoLibro? derecha)
        {
            this.derecha = derecha;
        }

        public NodoLibro? getPadre()
        {
            return this.padre;
        }

        public void setPadre(NodoLibro? padre)
        {
            this.padre = padre;
        }

        public long? getLlave()
        {
            return this.llave;
        }

        public void setLlave(long? llave)
        {
            this.llave = llave;
        }
    } 
}