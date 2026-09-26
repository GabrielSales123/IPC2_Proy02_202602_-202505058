using System.Diagnostics;
using System.Text;
using BiblioSystemV2.Models;

namespace BiblioSystemV2.Services;

public sealed class GeneradorGraphviz
{
    public Task<string> GenerarArbolCategoriasAsync(Categoria categoria, CancellationToken cancellationToken)
    {
        StringBuilder dot = new StringBuilder();
        dot.AppendLine("digraph Categorias {");
        dot.AppendLine("  graph [rankdir=TB, bgcolor=\"transparent\", nodesep=0.35, ranksep=0.55];");
        dot.AppendLine("  node [shape=box, style=\"rounded,filled\", fillcolor=\"#e5f2ed\", color=\"#176b59\", fontname=\"Arial\"];");
        dot.AppendLine("  edge [color=\"#647270\", arrowsize=0.7];");

        int nextId = 0;
        string rootId = CrearNodoCategoria(dot, categoria, ref nextId);
        AgregarSubcategorias(dot, categoria, rootId, ref nextId);
        dot.AppendLine("}");
        return RenderizarSvgAsync(dot.ToString(), cancellationToken);
    }

    public Task<string> GenerarArbolLibrosAsync(Categoria categoria, CancellationToken cancellationToken)
    {
        StringBuilder dot = new StringBuilder();
        dot.AppendLine("digraph Libros {");
        dot.AppendLine("  graph [rankdir=TB, bgcolor=\"transparent\", nodesep=0.3, ranksep=0.5];");
        dot.AppendLine("  node [shape=box, style=\"rounded,filled\", fillcolor=\"#fff2dc\", color=\"#a36519\", fontname=\"Arial\"];");
        dot.AppendLine("  edge [color=\"#647270\", arrowsize=0.7];");

        NodoLibro? raiz = categoria.getLibros()?.getRaiz();
        if (raiz == null)
        {
            dot.AppendLine("  vacio [label=\"Sin libros en esta categoría\"];");
        }
        else
        {
            AgregarNodosLibro(dot, raiz);
        }

        dot.AppendLine("}");
        return RenderizarSvgAsync(dot.ToString(), cancellationToken);
    }

    public static Categoria? BuscarCategoria(Categoria actual, string nombre)
    {
        if (string.Equals(actual.getNombre(), nombre, StringComparison.Ordinal))
        {
            return actual;
        }

        return BuscarSubcategoria(actual.getSubcategorias()?.getRaiz(), nombre);
    }

    private static Categoria? BuscarSubcategoria(NodoCategoria? nodo, string nombre)
    {
        if (nodo == null)
        {
            return null;
        }

        Categoria? encontrada = BuscarSubcategoria(nodo.getIzquierda(), nombre);
        if (encontrada != null)
        {
            return encontrada;
        }

        if (nodo.getCategoria() is Categoria categoria)
        {
            encontrada = BuscarCategoria(categoria, nombre);
            if (encontrada != null)
            {
                return encontrada;
            }
        }

        return BuscarSubcategoria(nodo.getDerecha(), nombre);
    }

    private static string CrearNodoCategoria(StringBuilder dot, Categoria categoria, ref int nextId)
    {
        string id = $"categoria{nextId++}";
        dot.Append("  ").Append(id).Append(" [label=\"")
            .Append(Escapar(categoria.getNombre() ?? string.Empty)).AppendLine("\"];");
        return id;
    }

    private static void AgregarSubcategorias(
        StringBuilder dot,
        Categoria categoria,
        string idPadre,
        ref int nextId)
    {
        AgregarNodosCategoria(dot, categoria.getSubcategorias()?.getRaiz(), idPadre, ref nextId);
    }

    private static void AgregarNodosCategoria(
        StringBuilder dot,
        NodoCategoria? nodo,
        string idPadre,
        ref int nextId)
    {
        if (nodo == null)
        {
            return;
        }

        AgregarNodosCategoria(dot, nodo.getIzquierda(), idPadre, ref nextId);
        if (nodo.getCategoria() is Categoria subcategoria)
        {
            string id = CrearNodoCategoria(dot, subcategoria, ref nextId);
            dot.Append("  ").Append(idPadre).Append(" -> ").Append(id).AppendLine(";");
            AgregarSubcategorias(dot, subcategoria, id, ref nextId);
        }

        AgregarNodosCategoria(dot, nodo.getDerecha(), idPadre, ref nextId);
    }

    private static void AgregarNodosLibro(StringBuilder dot, NodoLibro? nodo)
    {
        if (nodo == null)
        {
            return;
        }

        Libro? libro = nodo.getLibro();
        if (libro == null)
        {
            return;
        }

        string id = $"libro{libro.getISBN()}";
        string label = $"ISBN: {libro.getISBN()}\\n{Escapar(libro.getTitulo() ?? string.Empty)}\\n{Escapar(libro.getAutor() ?? string.Empty)}";
        dot.Append("  ").Append(id).Append(" [label=\"").Append(label).AppendLine("\"];");

        AgregarAristaLibro(dot, nodo.getIzquierda(), id, "izquierda");
        AgregarAristaLibro(dot, nodo.getDerecha(), id, "derecha");
        AgregarNodosLibro(dot, nodo.getIzquierda());
        AgregarNodosLibro(dot, nodo.getDerecha());
    }

    private static void AgregarAristaLibro(StringBuilder dot, NodoLibro? hijo, string idPadre, string lado)
    {
        if (hijo?.getLibro() is Libro libroHijo)
        {
            string idHijo = $"libro{libroHijo.getISBN()}";
            dot.Append("  ").Append(idPadre).Append(" -> ").Append(idHijo)
                .Append(" [label=\"").Append(lado).AppendLine("\"];");
        }
    }

    private static string Escapar(string valor)
    {
        return valor.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal)
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal);
    }

    private static async Task<string> RenderizarSvgAsync(string dot, CancellationToken cancellationToken)
    {
        ProcessStartInfo inicio = new ProcessStartInfo
        {
            FileName = "dot",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        inicio.ArgumentList.Add("-Tsvg");

        using Process proceso = new Process { StartInfo = inicio };
        try
        {
            if (!proceso.Start())
            {
                throw new InvalidOperationException("No fue posible iniciar Graphviz.");
            }

            Task<string> salida = proceso.StandardOutput.ReadToEndAsync(cancellationToken);
            Task<string> errores = proceso.StandardError.ReadToEndAsync(cancellationToken);
            await proceso.StandardInput.WriteAsync(dot.AsMemory(), cancellationToken);
            proceso.StandardInput.Close();
            await proceso.WaitForExitAsync(cancellationToken);

            string svg = await salida;
            string error = await errores;
            if (proceso.ExitCode != 0)
            {
                throw new InvalidOperationException($"Graphviz no pudo generar el gráfico: {error}");
            }

            return svg;
        }
        catch (System.ComponentModel.Win32Exception excepcion)
        {
            throw new InvalidOperationException("Graphviz no está instalado o 'dot' no está en PATH.", excepcion);
        }
    }
}