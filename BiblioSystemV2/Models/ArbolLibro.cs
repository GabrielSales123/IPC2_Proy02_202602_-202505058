
namespace BiblioSystemV2.Models
{
    public class ArbolLibro
    {
        private NodoLibro? raiz;
        public ArbolLibro()
        {
            this.raiz = null;
        }

        public void insertar(Libro libro)
        {
            NodoLibro nuevoNodo = new NodoLibro(libro);
            if (this.raiz == null)
            {
                this.raiz = nuevoNodo;
                return; 
            }
            NodoLibro aux = this.raiz;
            recursividadInsertar(aux, nuevoNodo);
        }

        public void recursividadInsertar(NodoLibro? aux, NodoLibro? nuevoNodo)
        {
            if (nuevoNodo == null || aux == null)
            {
                return;
            }
                int comparacion = nuevoNodo.getLlave().GetValueOrDefault().CompareTo(aux.getLlave().GetValueOrDefault());
            if (comparacion == 0)
            {
                return;
            }
            else if (comparacion < 0)
            {
                if (aux.getIzquierda() == null)
                {
                    aux.setIzquierda(nuevoNodo);
                    nuevoNodo.setPadre(aux);
                    return;
                }
                else
                {
                    recursividadInsertar(aux.getIzquierda(), nuevoNodo);
                }
            }
            else
            {
                if (aux.getDerecha() == null)
                {
                    aux.setDerecha(nuevoNodo);
                    nuevoNodo.setPadre(aux);
                    return;
                }
                else
                {
                    recursividadInsertar(aux.getDerecha(), nuevoNodo);
                }
            }
        }

        public NodoLibro? buscar(long llave)
        {
            NodoLibro? aux = this.raiz;
            if (aux == null)
            {
                return null;
            }
            else
            {
                return recursividadBuscar(aux, llave);
            }
        }
        public NodoLibro? getRaiz()
        {
            return this.raiz;
        }
        public NodoLibro? recursividadBuscar(NodoLibro? aux, long llave)
        {
            if (aux == null)
            {
                return null;
            }

            int comparacion = llave.CompareTo(aux.getLlave().GetValueOrDefault());
            if (comparacion == 0)
            {
                return aux;
            }
            else if (comparacion < 0)
            {
                if (aux.getIzquierda() == null)
                {
                    return null;
                }
                else
                {
                    return recursividadBuscar(aux.getIzquierda(), llave);
                }
            }
            else
            {
                if (aux.getDerecha() == null)
                {
                    return null;
                }
                else
                {
                    return recursividadBuscar(aux.getDerecha(), llave);
                }
            }
        }

    }
}