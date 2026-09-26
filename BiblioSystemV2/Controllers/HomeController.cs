using System.Diagnostics;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using BiblioSystemV2.Models;
using BiblioSystemV2.Services;

namespace BiblioSystemV2.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    private const long MaxXmlUploadBytes = 10 * 1024 * 1024;

    [HttpPost]
    [RequestSizeLimit(MaxXmlUploadBytes + 64 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxXmlUploadBytes)]
    public IActionResult CargarCatalogo(IFormFile? archivo)
    {
        if (archivo == null || archivo.Length == 0)
        {
            return BadRequest(new { mensaje = "Selecciona un archivo XML para cargar el catálogo." });
        }

        if (!string.Equals(Path.GetExtension(archivo.FileName), ".xml", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { mensaje = "El archivo debe tener extensión .xml." });
        }

        if (archivo.Length > MaxXmlUploadBytes)
        {
            return StatusCode(StatusCodes.Status413PayloadTooLarge, new { mensaje = "El archivo no puede superar 10 MB." });
        }

        try
        {
            using Stream contenido = archivo.OpenReadStream();
            Catalogo catalogo = new LecturaXML().LeerCatalogo(contenido);
            Categoria? raiz = catalogo.getCategorias()?.getRaiz()?.getCategoria();
            return raiz == null ? BadRequest(new { mensaje = "El XML no contiene un catálogo válido." }) : Json(ConvertirCategoria(raiz));
        }
        catch (InvalidDataException excepcion)
        {
            return BadRequest(new { mensaje = excepcion.Message });
        }
        catch (XmlException)
        {
            return BadRequest(new { mensaje = "El archivo XML no tiene un formato válido." });
        }
    }

    [HttpPost]
    [RequestSizeLimit(MaxXmlUploadBytes + 64 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxXmlUploadBytes)]
    public async Task<IActionResult> GenerarGrafico(
        IFormFile? archivo,
        string? tipo,
        string? categoria,
        CancellationToken cancellationToken)
    {
        if (archivo == null || archivo.Length == 0)
        {
            return BadRequest(new { mensaje = "Carga un archivo XML antes de generar el gráfico." });
        }

        if (!string.Equals(Path.GetExtension(archivo.FileName), ".xml", StringComparison.OrdinalIgnoreCase)
            || archivo.Length > MaxXmlUploadBytes)
        {
            return BadRequest(new { mensaje = "Selecciona un archivo XML válido de hasta 10 MB." });
        }

        if (string.IsNullOrWhiteSpace(categoria))
        {
            return BadRequest(new { mensaje = "Selecciona la categoría que quieres visualizar." });
        }

        try
        {
            using Stream contenido = archivo.OpenReadStream();
            Catalogo catalogo = new LecturaXML().LeerCatalogo(contenido);
            Categoria? raiz = catalogo.getCategorias()?.getRaiz()?.getCategoria();
            Categoria? seleccionada = raiz == null ? null : GeneradorGraphviz.BuscarCategoria(raiz, categoria);
            if (seleccionada == null)
            {
                return BadRequest(new { mensaje = $"No se encontró la categoría '{categoria}' en el XML." });
            }

            GeneradorGraphviz generador = new GeneradorGraphviz();
            string svg = tipo switch
            {
                "categorias" => await generador.GenerarArbolCategoriasAsync(seleccionada, cancellationToken),
                "libros" => await generador.GenerarArbolLibrosAsync(seleccionada, cancellationToken),
                _ => string.Empty
            };

            return string.IsNullOrEmpty(svg)
                ? BadRequest(new { mensaje = "El tipo de gráfico solicitado no es válido." })
                : Content(svg, "image/svg+xml", System.Text.Encoding.UTF8);
        }
        catch (InvalidDataException excepcion)
        {
            return BadRequest(new { mensaje = excepcion.Message });
        }
        catch (XmlException)
        {
            return BadRequest(new { mensaje = "El archivo XML no tiene un formato válido." });
        }
        catch (InvalidOperationException excepcion)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = excepcion.Message });
        }
    }

    private static CategoriaNavegador ConvertirCategoria(Categoria categoria)
    {
        return new CategoriaNavegador(
            categoria.getNombre() ?? string.Empty,
            ObtenerLibros(categoria.getLibros()?.getRaiz()),
            ObtenerSubcategorias(categoria.getSubcategorias()?.getRaiz()));
    }

    private static List<CategoriaNavegador> ObtenerSubcategorias(NodoCategoria? nodo)
    {
        if (nodo == null)
        {
            return new List<CategoriaNavegador>();
        }

        List<CategoriaNavegador> categorias = ObtenerSubcategorias(nodo.getIzquierda());
        if (nodo.getCategoria() is Categoria categoria)
        {
            categorias.Add(ConvertirCategoria(categoria));
        }

        categorias.AddRange(ObtenerSubcategorias(nodo.getDerecha()));
        return categorias;
    }

    private static List<LibroNavegador> ObtenerLibros(NodoLibro? nodo)
    {
        if (nodo == null)
        {
            return new List<LibroNavegador>();
        }

        List<LibroNavegador> libros = ObtenerLibros(nodo.getIzquierda());
        if (nodo.getLibro() is Libro libro)
        {
            libros.Add(new LibroNavegador(
                libro.getISBN(),
                libro.getTitulo() ?? string.Empty,
                libro.getAutor() ?? string.Empty,
                libro.getCategoria() ?? string.Empty));
        }

        libros.AddRange(ObtenerLibros(nodo.getDerecha()));
        return libros;
    }

    private sealed record CategoriaNavegador(
        string Nombre,
        List<LibroNavegador> Libros,
        List<CategoriaNavegador> Subcategorias);

    private sealed record LibroNavegador(int Isbn, string Titulo, string Autor, string Categoria);

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
