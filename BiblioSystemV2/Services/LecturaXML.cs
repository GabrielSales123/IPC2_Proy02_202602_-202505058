using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using BiblioSystemV2.Models;

namespace BiblioSystemV2.Services
{
    public class LecturaXML
    {
        private readonly string rutaArchivo;

        public LecturaXML(string? rutaArchivo = null)
        {
            this.rutaArchivo = rutaArchivo ?? Path.Combine(AppContext.BaseDirectory, "Services", "entrada_100.xml");
        }

        public Catalogo LeerCatalogo()
        {
            XmlReaderSettings configuracion = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null
            };

            using XmlReader lector = XmlReader.Create(this.rutaArchivo, configuracion);
            XDocument documento = XDocument.Load(lector);
            XElement raizXml = documento.Root ?? throw new InvalidDataException("El documento XML no tiene un elemento raiz.");
            XElement listaCategorias = raizXml.Element("listaCategorias")
                ?? throw new InvalidDataException("El documento no contiene listaCategorias.");
            XElement listaLibros = raizXml.Element("listaLibros")
                ?? throw new InvalidDataException("El documento no contiene listaLibros.");

            Dictionary<string, Categoria> categorias = new Dictionary<string, Categoria>(StringComparer.Ordinal);
            List<(XElement Elemento, string Nombre, string? Padre)> definiciones = new List<(XElement, string, string?)>();

            foreach (XElement elemento in listaCategorias.Elements("categoria"))
            {
                string nombre = elemento.Value.Trim();
                if (nombre.Length == 0)
                {
                    throw new InvalidDataException("Se encontro una categoria sin nombre.");
                }

                string? padre = (string?)elemento.Attribute("padre");
                padre = string.IsNullOrWhiteSpace(padre) ? null : padre.Trim();
                if (!categorias.TryAdd(nombre, new Categoria(nombre)))
                {
                    throw new InvalidDataException($"La categoria '{nombre}' esta duplicada.");
                }

                definiciones.Add((elemento, nombre, padre));
            }

            if (!categorias.TryGetValue("Catalogo", out Categoria? categoriaRaiz)
                || definiciones.Any(definicion => definicion.Nombre == "Catalogo" && definicion.Padre != null))
            {
                throw new InvalidDataException("Debe existir una categoria raiz 'Catalogo' sin atributo padre.");
            }

            categoriaRaiz.setLibros(new ArbolLibro());
            Catalogo catalogo = new Catalogo();
            catalogo.getCategorias()!.insertar(categoriaRaiz);

            foreach ((_, string nombre, string? nombrePadre) in definiciones)
            {
                if (nombre == "Catalogo")
                {
                    continue;
                }

                if (nombrePadre == null || !categorias.TryGetValue(nombrePadre, out Categoria? padre))
                {
                    throw new InvalidDataException($"No se encontro el padre de la categoria '{nombre}'.");
                }

                Categoria categoria = categorias[nombre];
                categoria.setPadre(padre);
                categoria.setLibros(new ArbolLibro());

                ArbolCategorias subcategorias = padre.getSubcategorias() ?? new ArbolCategorias();
                padre.setSubcategorias(subcategorias);
                subcategorias.insertar(categoria);
            }

            foreach (XElement elementoLibro in listaLibros.Elements("libro"))
            {
                int isbn = LeerEnteroRequerido(elementoLibro, "ISBN");
                string titulo = LeerTextoRequerido(elementoLibro, "titulo");
                string autor = LeerTextoRequerido(elementoLibro, "autor");
                string nombreCategoria = LeerTextoRequerido(elementoLibro, "categoria");

                if (!categorias.TryGetValue(nombreCategoria, out Categoria? categoria))
                {
                    throw new InvalidDataException($"El libro '{titulo}' referencia la categoria inexistente '{nombreCategoria}'.");
                }

                categoria.getLibros()!.insertar(new Libro(isbn, titulo, autor, nombreCategoria));
            }

            return catalogo;
        }

        private static int LeerEnteroRequerido(XElement elemento, string nombre)
        {
            string valor = LeerTextoRequerido(elemento, nombre);
            if (!int.TryParse(valor, NumberStyles.Integer, CultureInfo.InvariantCulture, out int resultado))
            {
                throw new InvalidDataException($"El valor '{valor}' de {nombre} no es un entero valido.");
            }

            return resultado;
        }

        private static string LeerTextoRequerido(XElement elemento, string nombre)
        {
            string? valor = elemento.Element(nombre)?.Value.Trim();
            if (string.IsNullOrEmpty(valor))
            {
                throw new InvalidDataException($"Falta el valor de {nombre} en un elemento del documento.");
            }

            return valor;
        }
    }
}