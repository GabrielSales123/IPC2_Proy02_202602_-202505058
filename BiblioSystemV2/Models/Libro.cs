
namespace BiblioSystemV2.Models
{
    public class Libro
    {
        private int ISBN;
        private string? titulo;
        private string? autor;
        private string? categoria;

        public Libro(int ISBN, string titulo, string autor, string categoria)
        {
            this.ISBN = ISBN;
            this.titulo = titulo;
            this.autor = autor;
            this.categoria = categoria;
        }

        public int getISBN()
        {
            return this.ISBN;
        }

        public void setISBN(int ISBN)
        {
            this.ISBN = ISBN;
        }

        public string? getTitulo()
        {
            return this.titulo;
        }

        public void setTitulo(string? titulo)
        {
            this.titulo = titulo;
        }

        public string? getAutor()
        {
            return this.autor;
        }

        public void setAutor(string? autor)
        {
            this.autor = autor;
        }

        public string? getCategoria()
        {
            return this.categoria;
        }

        public void setCategoria(string? categoria)
        {
            this.categoria = categoria;
        }
    }
}