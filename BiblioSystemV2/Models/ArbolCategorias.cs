
namespace BiblioSystemV2.Models
{
    public class ArbolCategorias
    {
        private NodoCategoria? raiz;

        public ArbolCategorias()
        {
            this.raiz = null;
        }

        public void insertar(Categoria categoria)
        {
            NodoCategoria nuevoNodo = new NodoCategoria(categoria);
            if (this.raiz == null)
            {
                this.raiz = nuevoNodo;
                return;
            }
            NodoCategoria aux = this.raiz;
            recursividadInsertar(aux, nuevoNodo);
        }

        public void recursividadInsertar(NodoCategoria? aux, NodoCategoria? nuevoNodo)
        {
            if (nuevoNodo == null || aux == null)
            {
                return;
            }
            int comparacion = (nuevoNodo.getLlave() ?? string.Empty).CompareTo(aux.getLlave() ?? string.Empty);
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

        public NodoCategoria? buscar(string llave)
        {
            NodoCategoria? aux = this.raiz;
            if (aux == null)
            {
                return null;
            }
            else
            {
                return recursividadBuscar(aux, llave);
            }
        }

        public NodoCategoria? getRaiz()
        {
            return this.raiz;
        }

        public NodoCategoria? recursividadBuscar(NodoCategoria? aux, string llave)
        {
            if (aux == null)
            {
                return null;
            }

            int comparacion = llave.CompareTo(aux.getLlave() ?? string.Empty);
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