
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

        public bool eliminar(long isbn)
        {
            NodoLibro? nodo = buscar(isbn);
            if (nodo == null)
            {
                return false;
            }

            if (nodo.getIzquierda() == null)
            {
                ReemplazarNodo(nodo, nodo.getDerecha());
            }
            else if (nodo.getDerecha() == null)
            {
                ReemplazarNodo(nodo, nodo.getIzquierda());
            }
            else
            {
                NodoLibro sucesor = ObtenerMinimo(nodo.getDerecha()!);
                if (sucesor.getPadre() != nodo)
                {
                    ReemplazarNodo(sucesor, sucesor.getDerecha());
                    sucesor.setDerecha(nodo.getDerecha());
                    sucesor.getDerecha()?.setPadre(sucesor);
                }

                ReemplazarNodo(nodo, sucesor);
                sucesor.setIzquierda(nodo.getIzquierda());
                sucesor.getIzquierda()?.setPadre(sucesor);
            }

            nodo.setPadre(null);
            nodo.setIzquierda(null);
            nodo.setDerecha(null);
            return true;
        }

        private static NodoLibro ObtenerMinimo(NodoLibro nodo)
        {
            while (nodo.getIzquierda() != null)
            {
                nodo = nodo.getIzquierda()!;
            }

            return nodo;
        }

        private void ReemplazarNodo(NodoLibro nodo, NodoLibro? reemplazo)
        {
            NodoLibro? padre = nodo.getPadre();
            if (padre == null)
            {
                this.raiz = reemplazo;
            }
            else if (padre.getIzquierda() == nodo)
            {
                padre.setIzquierda(reemplazo);
            }
            else
            {
                padre.setDerecha(reemplazo);
            }

            reemplazo?.setPadre(padre);
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